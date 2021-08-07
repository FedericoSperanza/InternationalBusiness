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

namespace InternationalBusiness.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        CurrencyService _currencyService = new CurrencyService();
        private readonly IConfiguration Configuration;
        public CurrencyController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET: api/Currencies
        [HttpGet]
        public async Task<CustomResponse<List<Currency>>> Get()
        {
            var myKeyValue = Configuration["EndPoints:CurrenciesAPI"];
            var resp = await _currencyService.GetAllCurrencies(myKeyValue);
            return resp;
        }

        // GET: api/Currencies/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
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
