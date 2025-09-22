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

namespace ServicesGateManagment.Server;

public class VehicleInquireService : IVehicleInquireService
{
    private readonly ApiService _httpClient;
    private readonly ILogger<VehicleInquireService> _logger;
    private readonly string _dataDirectory;
    private readonly IWebHostEnvironment _env;
    private ApplicationDbContext _db;
    public VehicleInquireService(ApiService httpClient, ILogger<VehicleInquireService> logger, IWebHostEnvironment env, ApplicationDbContext db)
    {
        _httpClient = httpClient;
        _logger = logger;
        _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "InquireData");
        _env = env;
        _db = db;
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
        //throw new NotImplementedException();
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
            if (vehicle!=null)
            {

                var now = DateTime.UtcNow;
                var hasAccess = true;
                var inBlackList = false;
                List<PropertyVehicle> grants = new();

                //if (vehicle is not null)
                //{
                //    // استخراج PropertiesVehicles از Properties در Vehicle
                //    grants = vehicles.SelectMany(v => v.Properties)
                //        .Where(x => x.VehicleId == vehicle.Id)
                //        .Where(x => x.Status == PropertyVehicleStatus.Approved)
                //        .Where(x => !x.ExpireDate.HasValue || x.ExpireDate.Value > now)
                //        .Where(x => !x.StartDate.HasValue || x.StartDate.Value < now)
                //        .ToList();

                //    var dayOfWeek = ((int)now.DayOfWeek + 1) % 7;


                //    // بررسی دسترسی (بدون Calendar، زیرا در JSON وجود ندارد)
                //    // دسترسی به Property در هر grant
                //    foreach (PropertyVehicle grant in grants)
                //    {
                //        if (grant.Property != null) // اطمینان از اینکه Property null نیست
                //        {
                //            // اینجا می‌توانید به فیلدهای Property دسترسی پیدا کنید
                //            var propertyId = grant.Property.Id;
                //            var propertyTitle = grant.Property.Title;
                //            // مثال: لاگ کردن اطلاعات Property
                //            _logger.LogInformation($"Property for Vehicle {vehicle.Id}: ID={propertyId}, Title={propertyTitle}");

                //            hasAccess = true; // فرض می‌کنیم اگر Property وجود دارد، دسترسی مجاز است
                //            grant.LastInquireDate = now;
                //        }
                //    }

                //    // بررسی Blacklist
                //    inBlackList = gate.Type switch
                //    {
                //        GateType.Exit => vehicle.IsExitBlacklisted,
                //        GateType.Entrance => vehicle.IsEntranceBlacklisted,
                //        _ => false
                //    };
                //}
                //else
                //{
                //    //vehicle = vehicleWithPlate;
                //    //vehicle.Color = car.CarColor;
                //    //vehicle.Type = VehicleType.Sedan;
                //    //vehicle.CreatedUtc = now;
                //    //vehicle.LastModifiedUtc = now;
                //    //vehicle.Id = vehicles.Any() ? vehicles.Max(x => x.Id) + 1 : 1;
                //    //vehicles.Add(vehicle);
                //}

                // بررسی سیاست Gate
                var gateValidation = !inBlackList && (!gate.NeedValidAccess || hasAccess);
                var lastGrant = grants.OrderByDescending(x => x.PropertyId).LastOrDefault();

                // ایجاد VehicleInquire
                //var entity = new VehicleInquire
                //{
                //    Plate = plaque.Plaque,
                //    Color = car.CarColor,
                //    Class = car.CarClass,
                //    GateId = gate.Id,
                //    VehicleId = 0,
                //    Type = gate.InquireType,
                //    GateValidation = gateValidation,
                //    ReferenceId = request.ReferenceId, // اصلاح: استفاده از request.ReferenceId
                //    PropertyVehicleId = null,//PropertyVehicleId = lastGrant?.Id,
                //    Result = new VehicleInquireResult
                //    {
                //        HasValidAccess = hasAccess,
                //        GateValidation = gateValidation,
                //        ArmAction = gateValidation ? gate.ArmActionOnSuccess : gate.ArmActionOnFailed,
                //        InBlackList = inBlackList
                //    },
                //    CreatedUtc = now
                //};

                //// بررسی ورود غیرمجاز
                //if (gate.CheckUnauthorizedEntry && !hasAccess)
                //{
                //    entity.Result.UnAuthorizedEntry = true;

                //    var latestEntrance = vehicleInquires
                //        .Where(x => x.VehicleId == vehicle.Id)
                //        .Where(x => x.Gate.Type == GateType.Entrance)
                //        .Where(x => x.CreatedUtc > now.AddMinutes(-1))
                //        .OrderByDescending(x => x.Id)
                //        .FirstOrDefault();

                //    if (latestEntrance != null)
                //    {
                //        vehicleExitBlacklist.Add(new VehicleExitBlacklist
                //        {
                //            Id = vehicleExitBlacklist.Any() ? vehicleExitBlacklist.Max(x => x.Id) + 1 : 1,
                //            VehicleId = vehicle.Id,
                //            Vehicle = vehicle,
                //            VehicleViolation = new VehicleViolation
                //            {
                //                Id = vehicleViolations.Any() ? vehicleViolations.Max(x => x.Id) + 1 : 1,
                //                Vehicle = vehicle,
                //                Type = VehicleViolationType.UnauthorizedEntry,
                //                VehicleInquire = latestEntrance
                //            }
                //        });

                //        latestEntrance.Result.UnAuthorizedEntry = true;
                //        entity.PreviousInquire = latestEntrance;
                //    }
                //}

                //// بررسی بازدیدکننده تأییدنشده
                //if (gate.Type == GateType.Exit && lastGrant is { Type: PropertyVehicleType.Visitor, VisitorConfirmStatus: VisitorConfirmStatus.Pending })
                //{
                //    vehicleViolations.Add(new VehicleViolation
                //    {
                //
                //        Vehicle = vehicle,
                //        Type = VehicleViolationType.NotConfirmedVisitor,
                //        PropertyVehicleId = lastGrant.Id,
                //        VehicleInquire = entity
                //    });

                //    entity.Result.NotConfirmedVisitor = true;
                //}

                // بررسی اقامت بیش از حد
                //if (gate.Type == GateType.Exit && !hasAccess)
                //{
                //    var access = vehicles.SelectMany(v => v.Properties)
                //        .Where(x => x.VehicleId == vehicle.Id)
                //        .Where(x => x.Status == PropertyVehicleStatus.Approved)
                //        .OrderBy(x => x.VehicleId)
                //        .LastOrDefault();

                //    if (access is not null)
                //    {
                //        var overstay = DateTime.Now - access.ExpireDate!.Value;
                //        switch (access.Type)
                //        {
                //            case PropertyVehicleType.Guest:
                //                if (overstay.TotalDays > 1)
                //                {
                //                    //vehicleViolations.Add(new VehicleViolation
                //                    //{
                //                    //    Id = vehicleViolations.Any() ? vehicleViolations.Max(x => x.Id) + 1 : 1,
                //                    //    Vehicle = vehicle,
                //                    //    Type = VehicleViolationType.Overstaying,
                //                    //    VehicleInquire = entity,
                //                    //    PropertyVehicleId = access.Id
                //                    //});
                //                }
                //                break;

                //            case PropertyVehicleType.Visitor:
                //                if (overstay.TotalHours > 1)
                //                {
                //                    //vehicleExitBlacklist.Add(new VehicleExitBlacklist
                //                    //{
                //                    //    Id = vehicleExitBlacklist.Any() ? vehicleExitBlacklist.Max(x => x.Id) + 1 : 1,
                //                    //    VehicleViolation = new VehicleViolation
                //                    //    {
                //                    //        Id = vehicleViolations.Any() ? vehicleViolations.Max(x => x.Id) + 1 : 1,
                //                    //        Vehicle = vehicle,
                //                    //        Type = VehicleViolationType.Overstaying,
                //                    //        VehicleInquire = entity,
                //                    //        PropertyVehicleId = access.Id
                //                    //    },
                //                    //    Vehicle = vehicle,
                //                    //    VehicleId = vehicle.Id
                //                    //});
                //                }
                //                break;
                //        }

                //        entity.Result.OverStayed = true;
                //    }
                //}

                // افزودن VehicleInquire به لیست
                //_db.Add(entity);
                //await _db.SaveChangesAsync(cancellationToken);

               

                return new VehicleInquireResultVm()
                {
                    HasValidAccess = hasAccess,
                    GateValidation = gateValidation,
                    ArmAction = gateValidation ? gate.ArmActionOnSuccess.ToString() : gate.ArmActionOnFailed.ToString(),
                    InBlackList = inBlackList,
                };
                //return _mapper.Map<VehicleInquireResultVm>(entity.Result);
            }
            ////save requestJson in DB
            var requestEntity = new VehicleInquireRequestJson
            {
                RequestData = JsonSerializer.Serialize(request),
                CreatedAt = DateTime.UtcNow,
                IsSent = false
            };

            _db.Add(requestEntity);
            await _db.SaveChangesAsync();
            return new VehicleInquireResultVm(){};
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
}