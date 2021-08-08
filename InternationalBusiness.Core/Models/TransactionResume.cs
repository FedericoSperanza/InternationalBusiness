using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternationalBusiness.Core.Models
{
    public class TransactionResume
    {
        public List<Transaction> transactionList {get; set;}
        public int totalAmount { get; set; }
    }
}
