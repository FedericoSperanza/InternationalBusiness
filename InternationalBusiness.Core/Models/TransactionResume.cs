using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternationalBusiness.Core.Models
{
    public class TransactionResume
    {
        public List<TranItemResume> transactionList {get; set;}
        public double totalAmount { get; set; }
    }
}
