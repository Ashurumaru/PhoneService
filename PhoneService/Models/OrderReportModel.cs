using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneService.Models
{
    internal class OrderReportModel
    {
        public DateTime? OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Payments { get; set; }
    }
}
