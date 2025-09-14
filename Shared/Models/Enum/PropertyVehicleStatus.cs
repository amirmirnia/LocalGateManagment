using System.ComponentModel;

namespace ServicesGateManagment.Shared.Enum
{
    public enum PropertyVehicleStatus
    {
        [Description("در حال بررسی")] Requested = 1,
        [Description("تایید شده")] Approved = 2,
        [Description("رد شده")] Denied = 3
    }
}
