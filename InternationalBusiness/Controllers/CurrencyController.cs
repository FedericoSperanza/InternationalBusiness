using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InternationalBusiness.Core.Currency;
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
        CurrencyService _currencyService = new CurrencyService();
        private readonly IConfiguration Configuration;
        private readonly string _currenciesApi;
        public CurrencyController(IConfiguration configuration)
        {
            Configuration = configuration;
            _currenciesApi = Configuration["EndPoints:CurrenciesAPI"];
        }

        // GET: api/Currencies
        [HttpGet]
        public async Task<CustomResponse<List<Currency>>> GetAllCurrencies()
        {               
            var resp = await _currencyService.GetAllCurrencies(_currenciesApi);
            if (resp.Data != null) { 
                //ToDo override bkCurrency file with updated data
                return resp;
            }
            else
                
            {
                CustomResponse<List<Currency>> response = new CustomResponse<List<Currency>>();
                try { 
                string json = System.IO.File.ReadAllText("bkpCurrency.json");
                var jsonCurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Currency>>(json);
                response.Data = jsonCurrencies;
                response.Message = "This Data is returned from backup file since the API Endpoint was down";
                return response;
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
            var resp = await _currencyService.GetCurrencyByType(_currenciesApi, currencyType);
            if(resp.Data != null)
            {
                return resp;
            }
            else
            {
                CustomResponse<Currency> response = new CustomResponse<Currency>();
                try {                
                string json = System.IO.File.ReadAllText("bkpCurrency.json");
                var jsonCurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Currency>>(json);
                var filteredCurrency = (from c in jsonCurrencies
                                        where c.@from == currencyType
                                        select new { c }).FirstOrDefault();

                response.Data = filteredCurrency.c;
                response.Message = "This Data is returned from backup file since the API Endpoint was down";
                return response;
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
