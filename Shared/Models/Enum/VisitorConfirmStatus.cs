using System.ComponentModel;

namespace ServicesGateManagment.Shared.Enum
{
    public enum VisitorConfirmStatus
    {
        [Description("در انتظار تایید")] Pending = 1,
        [Description("تایید شده")] Approved = 2,
        [Description("رد شده")] Denied = 3
    }
} 