using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Enum;
using ServicesGateManagment.Shared.Gates;
using ServicesGateManagment.Shared.Propertys;
using ServicesGateManagment.Shared.Vehicles;
using ServicesGateManagment.Shared.Models.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ServicesGateManagment.Shared.DBContext;
using System.Text.Json;
using ServicesGateManagment.Server.Handlers;
using ServicesGateManagment.Shared.Models.ViewModel.Vehicles;
using AutoMapper;
using System;

namespace ServicesGateManagment.Server;

public class VehicleInquireService : IVehicleInquireService
{
    private readonly ApiService _httpClient;
    private readonly ILogger<VehicleInquireService> _logger;
    private readonly string _dataDirectory;
    private readonly IWebHostEnvironment _env;
    private ApplicationDbContext _db;
    private IWebHostEnvironment _environment;
    private readonly IMapper _mapper;
    public VehicleInquireService(ApiService httpClient,
        ILogger<VehicleInquireService> logger,
        IWebHostEnvironment env, ApplicationDbContext db, IMapper mapper, IWebHostEnvironment environment)
    {
        _httpClient = httpClient;
        _logger = logger;
        _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "InquireData");
        _env = env;
        _db = db;
        _environment = environment;
        _mapper = mapper;
        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
        }
    }


    private readonly Dictionary<string, VehiclePlateLetter> _plaqueLetterMapping
           = new()
           {
                { "a", VehiclePlateLetter.Alef },
                { "b", VehiclePlateLetter.Be },
                { "c", VehiclePlateLetter.Sad },
                { "d", VehiclePlateLetter.Dal },
                { "e", VehiclePlateLetter.Zhe },
                { "f", VehiclePlateLetter.Fe },
                { "g", VehiclePlateLetter.Gaf },
                { "h", VehiclePlateLetter.He },
                { "i", VehiclePlateLetter.Eyn },
                { "j", VehiclePlateLetter.Jim },
                { "k", VehiclePlateLetter.Kaf },
                { "l", VehiclePlateLetter.Lam },
                { "m", VehiclePlateLetter.Mim },
                { "n", VehiclePlateLetter.Nun },
                { "o", VehiclePlateLetter.Se },
                { "p", VehiclePlateLetter.Pe },
                { "q", VehiclePlateLetter.Qaf },
                { "s", VehiclePlateLetter.Sin },
                { "t", VehiclePlateLetter.Te },
                { "u", VehiclePlateLetter.Shin },
                { "v", VehiclePlateLetter.Vav },
                { "w", VehiclePlateLetter.Ta },
                { "y", VehiclePlateLetter.Ye },
                { "z", VehiclePlateLetter.Ze }
           };

   
    public async Task<VehicleInquireResultVm> ProcessInquireAsync(CreateVehicleInquireRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation($"Processing vehicle inquire for gate: {request.Gate}, Reference: {request.ReferenceId}");

            // خواندن Vehicles از فایل JSON
            string vehicleFilePath = Path.Combine(_env.ContentRootPath, "FetchedData", "data.json");
            var jsonVehicle = await File.ReadAllTextAsync(vehicleFilePath, cancellationToken);
            List<VehicleJson> vehicles = System.Text.Json.JsonSerializer.Deserialize<List<VehicleJson>>(jsonVehicle) ?? new List<VehicleJson>();

            // خواندن Gates از فایل JSON
            string gateFilePath = Path.Combine(_env.ContentRootPath, "FetchedData", "Gate.json");
            var jsonGate = await File.ReadAllTextAsync(gateFilePath, cancellationToken);
            List<Gate> gates = System.Text.Json.JsonSerializer.Deserialize<List<Gate>>(jsonGate) ?? new List<Gate>();

            // یافتن Gate
            var gate = gates.FirstOrDefault(x => x.Guid == request.Gate) ??
                       throw new ValidationException("Gate Invalid gate id");

            if (gate.ObjectType != GateObjectType.Vehicle) throw new ValidationException("Invalid gate type");

            var car = request.Cars.FirstOrDefault() ?? throw new ValidationException("Cars Car not found!");
            var plaque = car.DetectedPlaques.FirstOrDefault() ?? throw new ValidationException("Plaque Plaque not found!");

            var vehicleWithPlate =await GetVehicleFromPlaque(plaque.Plaque);

            // جستجوی وسیله نقلیه با پلاک
            var vehicle = vehicleWithPlate.Letter == VehiclePlateLetter.Gaf
                ? vehicles.FirstOrDefault(x => x.PlatePart1 == vehicleWithPlate.PlatePart1 &&
                                              x.Letter == VehiclePlateLetter.Gaf &&
                                              x.PlatePart2 == vehicleWithPlate.PlatePart2)
                : vehicles.FirstOrDefault(x => x.PlatePart1 == vehicleWithPlate.PlatePart1 &&
                                              x.Letter == vehicleWithPlate.Letter &&
                                              x.PlatePart2 == vehicleWithPlate.PlatePart2 &&
                                              x.PlatePart3 == vehicleWithPlate.PlatePart3);
            var now = DateTime.UtcNow;
            var hasAccess = false;
            var inBlackList = false;
            if (vehicle!=null)
            {
                hasAccess = true;
                inBlackList = false; 
            }
            //سیاست گیت
            var gateValidation = !inBlackList && (!gate.NeedValidAccess || hasAccess);

            //save requestJson in DB
            var requestEntity = new VehicleInquireRequestJson
            {
                RequestData = JsonSerializer.Serialize(request),
                CreatedAt = DateTime.UtcNow,
                IsSent = false
            };

            _db.Add(requestEntity);
            await _db.SaveChangesAsync();

            return new VehicleInquireResultVm()
            {
                HasValidAccess = hasAccess,
                GateValidation = gateValidation,
                ArmAction = gateValidation ? gate.ArmActionOnSuccess.ToString() : gate.ArmActionOnFailed.ToString(),
                InBlackList = inBlackList,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing vehicle inquire");
            throw;
        }
    }

    public async Task<VehicleJson> GetVehicleFromPlaque(string plaque)
    {
        try
        {
            return new VehicleJson()
            {
                PlatePart1 = int.Parse(plaque.Substring(0, 2)),
                Letter =await GetPlateLetter(plaque.Substring(2, 1)),
                PlatePart2 = int.Parse(plaque.Substring(3, 3)),
                PlatePart3 = int.Parse(plaque.Substring(6, 2)),
            };
        }
        catch (Exception e)
        {
            throw new ValidationException("Plaque");
        }
    }

    public async Task<VehiclePlateLetter> GetPlateLetter(string plaqueLetter)
    {
        return _plaqueLetterMapping[plaqueLetter];
    }

    public async Task<List<VehicleInquireRequestJsonVM>> GetAllRequestVehicle()
    {
         var resualt = await _db.VehicleInquireRequestJson.ToListAsync();
        var finalmodel = _mapper.Map<List<VehicleInquireRequestJsonVM>>(resualt);
        return finalmodel;
    }

    public async Task<int> CountVehicleInFileJson(string fileName)
    {
        try
        {
           var filePath=Path.Combine(_environment.ContentRootPath, "FetchedData", fileName);
            // خواندن فایل JSON
            string jsonString = await File.ReadAllTextAsync(filePath);

            // تجزیه JSON به JsonDocument
            using JsonDocument document = JsonDocument.Parse(jsonString);

            // دسترسی به آرایه اصلی (RootElement باید آرایه باشد)
            if (document.RootElement.ValueKind == JsonValueKind.Array)
            {
                // شمردن تعداد عناصر آرایه
                return document.RootElement.EnumerateArray().Count();
            }

            return 0; // اگر JSON آرایه نباشد
        }
        catch (Exception ex)
        {
            Console.WriteLine($"خطا در خواندن فایل JSON: {ex.Message}");
            return -1; // یا مدیریت خطا به روش دلخواه
        }
    }
}