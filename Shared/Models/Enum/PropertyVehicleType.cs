using System.ComponentModel;

namespace ServicesGateManagment.Shared.Enum
{
    public enum PropertyVehicleType
    {
        [Description("مالک")] Owner = 1,
        [Description("بهره‌بردار")] Executive = 2,
        [Description("ساکن")] Member = 3,
        [Description("مهمان")] Guest = 4,
        [Description("ارباب رجوع")] Visitor = 5,
        [Description("تاکسی")] Taxi = 6,
        [Description("ملک")] Property = 7,
        [Description("خودروی سرویسی")] Service = 8
    }
}
