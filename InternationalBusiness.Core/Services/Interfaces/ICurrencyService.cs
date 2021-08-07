using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InternationalBusiness.Core.Models;
namespace InternationalBusiness.Core.Services.Interfaces
{
    interface ICurrencyService
    {
        Task<Models.CustomResponse<List<Models.Currency>>> GetAllCurrencies();
        Task<Models.CustomResponse<Models.Currency>> GetCurrencyByType(string currencyType);
    }
}
