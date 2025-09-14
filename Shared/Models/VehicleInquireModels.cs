using ServicesGateManagment.Shared.Enum;

namespace ServicesGateManagment.Shared;

// Request models matching GateAccessManagement structure exactly
public class CreateVehicleInquireRequest
{
    public long Timestamp { get; set; }
    public Guid Gate { get; set; }
    public string HotSpotId { get; set; } = string.Empty;
    public string CameraSnapshotBase64 { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public IList<InquireVehicleDto> Cars { get; set; } = new List<InquireVehicleDto>();
}

public class InquireVehicleDto
{
    public string CarClass { get; set; } = string.Empty;
    public string CarColor { get; set; } = string.Empty;
    public string CarImage { get; set; } = string.Empty;
    public IList<DetectionBoxDto> CarDetectionBox { get; set; } = new List<DetectionBoxDto>();
    public IList<DetectedPlatesDto> DetectedPlaques { get; set; } = new List<DetectedPlatesDto>();
}

public class DetectedPlatesDto
{
    public string Plaque { get; set; } = string.Empty;
    public string PlaqueImage { get; set; } = string.Empty;
    public IList<DetectionBoxDto> Rectangle { get; set; } = new List<DetectionBoxDto>();
    public string PlaqueType { get; set; } = string.Empty;
}

public class DetectionBoxDto
{
    public int X { get; set; }
    public int Y { get; set; }
}

// Response model matching GateAccessManagement structure
public class VehicleInquireResultVm
{
    public bool HasValidAccess { get; set; }
    public bool GateValidation { get; set; }
    public bool UnAuthorizedEntry { get; set; }
    public bool OverStayed { get; set; }
    public bool NotConfirmedVisitor { get; set; }
    public bool InBlackList { get; set; }
    public string ArmAction { get; set; } = string.Empty;

    //public GateArmAction ArmAction { get; set; }
}