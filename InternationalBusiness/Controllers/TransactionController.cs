using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InternationalBusiness.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using InternationalBusiness.Core.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace InternationalBusiness.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        TransactionService _transactionService;
        public TransactionController(TransactionService tranService)
        {
            _transactionService = tranService;
        }
        // GET: api/Transaction
        [HttpGet]
        public async Task<CustomResponse<List<TranItemResume>>> GetAllCurrencies()
        {
            var resp = await _transactionService.GetAllTransactions();
            if (resp.Data != null)
            {
                //ToDo override bkCurrency file with updated data
                return resp;
            }
            else
            {

                var res = await _transactionService.GetAllTransactionsBackup();
                return res;
            }

        }

        // GET: api/Transaction/5
        [HttpGet("{sku}", Name = "GetTransactionBysku")]
        public async Task<CustomResponse<TransactionResume>> GetTransactionBySku(string sku)
        {
            var resp = await _transactionService.GetTransactionBySku(sku);
            if (resp.Data != null)
            {
                return resp;
            }
            else
            {
                var res = await _transactionService.GetTransactionBySkuBackup(sku);
                return res;
            }
        }
    }
}
