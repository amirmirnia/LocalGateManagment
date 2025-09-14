using ServicesGateManagment.Shared.Common;
using ServicesGateManagment.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.Models.Vehicles
{
    public class VehicleInquireResult : BaseAuditableEntity
    {
        public bool HasValidAccess { get; set; }
        public bool GateValidation { get; set; }
        public bool UnAuthorizedEntry { get; set; }
        public bool OverStayed { get; set; }
        public bool NotConfirmedVisitor { get; set; }
        public bool InBlackList { get; set; }
        public GateArmAction ArmAction { get; set; }


        public int InquireId { get; set; }
        public VehicleInquire Inquire { get; set; } = null!;
    }
}
