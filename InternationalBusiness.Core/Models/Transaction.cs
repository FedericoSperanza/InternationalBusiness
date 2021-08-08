using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternationalBusiness.Core.Models
{
    public class Transaction
    {
        public string sku { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
    }
}
