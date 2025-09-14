using ServicesGateManagment.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.Models.Vehicles
{
    public class VehicleInquireSnapshot : BaseAuditableEntity
    {
        public string PlateImage { get; set; }
        public string CameraSnapshot { get; set; }
        public string ReferenceId { get; set; } = string.Empty;

        public int? InquireId { get; set; }
        public VehicleInquire? Inquire { get; set; }
    }
}
