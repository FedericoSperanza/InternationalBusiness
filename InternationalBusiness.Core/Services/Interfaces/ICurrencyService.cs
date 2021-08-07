using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InternationalBusiness.Core.Models;
namespace InternationalBusiness.Core.Services.Interfaces
{
    interface ICurrencyService
    {
        Task<Models.CustomResponse<List<Models.Currency>>> GetAllCurrencies(string endpointAPI);
        Task<Models.CustomResponse<Models.Currency>> GetCurrencyByType(string endpointAPI, string currencyType);
    }
}
