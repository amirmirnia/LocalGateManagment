using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.Models.Common
{
    public class FetchDataRequest
    {
        public string Endpoint { get; set; } = string.Empty;
        public string? RequestBody { get; set; } // اضافه کردن بدنه درخواست

    }
}
