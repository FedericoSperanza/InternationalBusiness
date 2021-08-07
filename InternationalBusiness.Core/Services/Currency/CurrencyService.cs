using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using InternationalBusiness.Core.Services.Interfaces;
using System.Linq;
namespace InternationalBusiness.Core.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private readonly string _currenciesApi;
        public CurrencyService(string currenciesAPI)
        {
            _currenciesApi = currenciesAPI;
        }

        public async Task<Models.CustomResponse<List<Models.Currency>>> GetAllCurrencies()
        {
            Models.CustomResponse<List<Models.Currency>> customResponse = new Models.CustomResponse<List<Models.Currency>>();
            try
            {
                var client = new HttpClient();
                HttpResponseMessage Res = await client.GetAsync(_currenciesApi);
                if (Res.IsSuccessStatusCode)
                {
                    var ObjResponse = Res.Content.ReadAsStringAsync().Result;
                    var serializedCurrencies = JsonConvert.DeserializeObject<List<Models.Currency>>(ObjResponse);
                    customResponse.Data = serializedCurrencies;
                    customResponse.isSuccess = true;                  
                    return customResponse;
                }
                else
                {
                    customResponse.Data = null;
                    customResponse.isSuccess = false;
                    return customResponse;
                }
            }
            catch (Exception e)
            {
                customResponse.ErrorMessage = e.Message;
                customResponse.Data = null;
                return customResponse;
            }
        }
        public async Task<Models.CustomResponse<Models.Currency>> GetCurrencyByType(string currencyType)
        {
            Models.CustomResponse<Models.Currency> customResponse = new Models.CustomResponse<Models.Currency>();
            try
            {
                var client = new HttpClient();
                HttpResponseMessage Res = await client.GetAsync(_currenciesApi);
                if (Res.IsSuccessStatusCode)
                {
                    var ObjResponse = Res.Content.ReadAsStringAsync().Result;
                    var serializedCurrencies = JsonConvert.DeserializeObject<List<Models.Currency>>(ObjResponse);
                    var filteredCurrency = (from c in serializedCurrencies
                                           where c.@from == currencyType
                                           select new { c }).FirstOrDefault();

                    customResponse.Data = filteredCurrency.c;
                    customResponse.isSuccess = true;
                    return customResponse;
                }
                else
                {
                    customResponse.Data = null;
                    customResponse.isSuccess = false;
                    return customResponse;
                }
            }
            catch (Exception e)
            {
                customResponse.ErrorMessage = e.Message;
                customResponse.Data = null;
                return customResponse;
            }

        }
    }
}
