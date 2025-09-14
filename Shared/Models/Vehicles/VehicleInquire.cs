using ServicesGateManagment.Shared.Common;
using ServicesGateManagment.Shared.Enum;
using ServicesGateManagment.Shared.Gates;
using ServicesGateManagment.Shared.Propertys;
using ServicesGateManagment.Shared.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.Models.Vehicles
{
    public class VehicleInquire : BaseAuditableEntity
    {
        public string Plate { get; set; }
        public string Color { get; set; }
        public string Class { get; set; }

        public int GateId { get; set; }
        public Gate Gate { get; set; } = null!;

        public int? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int? PropertyVehicleId { get; set; }
        public PropertyVehicle PropertyVehicle { get; set; } = null!;

        public VehicleInquireResult Result { get; set; } = null!;

        public ICollection<VehicleInquireSnapshot> Snapshots { get; set; } = new List<VehicleInquireSnapshot>();

        public bool GateValidation { get; set; }

        public InquireType Type { get; set; }

        public string ReferenceId { get; set; } = string.Empty;

        public int? PreviousInquireId { get; set; }
        public VehicleInquire PreviousInquire { get; set; } = null!;
    }
}
