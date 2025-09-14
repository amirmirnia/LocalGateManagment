using System.ComponentModel;

namespace ServicesGateManagment.Shared.Enum
{
    public enum PropertyState
    {
        [Description("متروکه")] Abandoned = 1, //متروکه
        [Description("در حال ساخت")] UnderConstruction = 2, //در حال ساخت
        [Description("در حال بهره‌برداری")] InOperation = 3, // در حال بهره برداری
        [Description("زمین خالی")] Undeveloped = 4, // زمین خالی
        [Description("بدهکار")] Indebted = 5, // بدهکار

        [Description("اجاره‌ای  بلند مدت")] LongTermRental = 6, //اجاره‌ای  بلند مدت

        [Description("اجاره‌ای کوتاه مدت")] ShortTermRental = 7 //اجاره‌ای کوتاه مدت
    }
}
