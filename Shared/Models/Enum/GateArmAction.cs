using System.ComponentModel;

namespace ServicesGateManagment.Shared.Enum
{
    public enum GateArmAction
    {
        [Description("باز و سپس بسته شود")] OpenThenClose = 1,
        [Description("باز شود")] Open = 2,
        [Description("بسته شود")] Close = 3
    }
}
