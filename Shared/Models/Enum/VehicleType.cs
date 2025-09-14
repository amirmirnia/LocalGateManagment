using System.ComponentModel;

namespace ServicesGateManagment.Shared.Enum
{
    public enum VehicleType
    {
        [Description("نامشخص")] None = 0,
        [Description("سواری")] Sedan = 1,
        [Description("اتوبوس")] Bus = 2,
        [Description("مینی بوس")] MiniBus = 3,
        [Description("باری")] Cargo = 4,
        [Description("ساختمانی")] Construction = 5
    }
}
