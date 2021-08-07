using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternationalBusiness.Core.Models
{
    public class Currency
    {
        public string from { get; set; }
        public string to { get; set; }
        public decimal rate { get; set; }
    }
}
