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
        public CurrencyController( CurrencyService currService)
        {
            _currencyService = currService;
        }

        // GET: api/Currency
        [HttpGet]
        public async Task<CustomResponse<List<Currency>>> GetAllCurrencies()
        {               
            var resp = await _currencyService.GetAllCurrencies();
            if (resp.Data != null) { 
                //ToDo override bkCurrency file with updated data
                return resp;
            }
            else             
            {
                CustomResponse<List<Currency>> response = new CustomResponse<List<Currency>>();
                try {
                    string json = System.IO.File.ReadAllText("bkpCurrency.json");
                    //ToDo config out name
                    var res = await _currencyService.GetAllCurrenciesBackup(json);
                    return res;
                }catch(Exception e)
                {
                    response.Message = "Error when trying to list all the currencies.";
                    response.ErrorMessage = e.Message;
                    response.Data = null;
                    return response;
                }
            }

        }

        // GET: api/Currencies/5
        [HttpGet("{currencyType}", Name = "Get")]
        public async Task<CustomResponse<Currency>> GetCurrencyByType(string currencyType)
        {
            var resp = await _currencyService.GetCurrencyByType(currencyType);
            if(resp.Data != null)
            {
                return resp;
            }
            else
            {
                CustomResponse<Currency> response = new CustomResponse<Currency>();
                try {                
                string json = System.IO.File.ReadAllText("bkpCurrency.json");
                var res = await _currencyService.GetCurrencyByTypeBackup(json,currencyType);
                return res;
                }catch(Exception e)
                {
                    response.Message = "Currency Not Found.";
                    response.ErrorMessage = e.Message;
                    response.Data = null;
                    return response;
                }
            }
        }

        // POST: api/Currencies
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Currencies/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
