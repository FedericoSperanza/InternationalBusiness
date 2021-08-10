using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InternationalBusiness.Core.Models;
namespace InternationalBusiness.Core.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<Models.CustomResponse<List<Models.Currency>>> GetAllCurrencies();
        Task<Models.CustomResponse<List<Models.Currency>>> GetCurrencyByType(string currencyType);
        Task<Models.CustomResponse<List<Models.Currency>>> GetAllCurrenciesBackup();
        Task<Models.CustomResponse<List<Models.Currency>>> GetCurrencyByTypeBackup(string currencyType);
    }
}
