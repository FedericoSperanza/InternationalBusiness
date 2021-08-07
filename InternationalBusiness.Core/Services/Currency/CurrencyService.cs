using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using InternationalBusiness.Core.Services.Interfaces;

namespace InternationalBusiness.Core.Currency
{
    public class CurrencyService : ICurrencyService
    {

        public CurrencyService()
        {
        }

        public async Task<Models.CustomResponse<List<Models.Currency>>> GetAllCurrencies(string endpointAPI)
        {
            Models.CustomResponse<List<Models.Currency>> customResponse = new Models.CustomResponse<List<Models.Currency>>();
            try
            {
                var client = new HttpClient();
                HttpResponseMessage Res = await client.GetAsync(endpointAPI);
                if (Res.IsSuccessStatusCode)
                {                  
                    List<Models.Currency> currencyList = new List<Models.Currency>();
                    var ObjResponse = Res.Content.ReadAsStringAsync().Result;
                    var serialized = JsonConvert.DeserializeObject<List<Models.Currency>>(ObjResponse);
                    customResponse.Data = serialized;
                    return customResponse;
                }
                else
                {
                    /////ToDo handling when API is down Get File Json locally
                    customResponse.ErrorMessage = "ToDo Get From File";
                    return customResponse;
                }
            }
            catch (Exception e)
            {
                customResponse.ErrorMessage = e.Message;
                return customResponse;
            }
        }
    }
}
