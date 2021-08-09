using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InternationalBusiness.Core.Models;
namespace InternationalBusiness.Core.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Models.CustomResponse<List<Models.TranItemResume>>> GetAllTransactions();
        Task<Models.CustomResponse<Models.TransactionResume>> GetTransactionBySku(string sku);
        Task<Models.CustomResponse<List<Models.TranItemResume>>> GetAllTransactionsBackup();
        Task<Models.CustomResponse<Models.TransactionResume>> GetTransactionBySkuBackup(string sku);
    }
}
