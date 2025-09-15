using ServicesGateManagment.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.Models.Vehicles
{
    public class VehicleInquireRequestJson : BaseAuditableEntity
    {
        public int Id { get; set; }
        public string RequestData { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
