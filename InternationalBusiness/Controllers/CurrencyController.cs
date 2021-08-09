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
    [Route("api/[controller]/")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        CurrencyService _currencyService;
        public CurrencyController(CurrencyService currService)
        {
            _currencyService = currService;
        }

        // GET: api/Currency
        [HttpGet]
        public async Task<CustomResponse<List<Currency>>> GetAllCurrencies()
        {
            var resp = await _currencyService.GetAllCurrencies();
            if (resp.Data != null)
            {
                //ToDo override bkCurrency file with updated data
                return resp;
            }
            else
            {
                var res = await _currencyService.GetAllCurrenciesBackup();
                return res;
            }

        }

        // GET: api/Currencies/5
        [HttpGet("{currencyType}", Name = "Get")]
        public async Task<CustomResponse<Currency>> GetCurrencyByType(string currencyType)
        {
            var resp = await _currencyService.GetCurrencyByType(currencyType);
            if (resp.Data != null)
            {
                return resp;
            }
            else
            {
                var res = await _currencyService.GetCurrencyByTypeBackup(currencyType);
                return res;
            }
        }
    }
}
