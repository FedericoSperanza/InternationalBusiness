using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using InternationalBusiness.Core.Services.Interfaces;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace InternationalBusiness.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly string _transactionsAPI;
        Core.Services.CurrencyService _currencyService;
        private readonly IConfiguration _configuration;
        List<Models.TranItemResume> inEuroList = new List<Models.TranItemResume>();
        private readonly string _bkpFilePath;
        public TransactionService(IConfiguration config, CurrencyService currService)
        {
            _configuration = config;
            _transactionsAPI = _configuration["EndPoints:TransactionsAPI"];
            _currencyService = currService;
            _bkpFilePath = _configuration["BackupFilesPaths:TransactionsPath"];
        }

        public async Task<Models.CustomResponse<List<Models.TranItemResume>>> GetAllTransactions()
        {
            Models.CustomResponse<List<Models.TranItemResume>> customResponse = new Models.CustomResponse<List<Models.TranItemResume>>();
            try
            {
                var client = new HttpClient();
                HttpResponseMessage Res = await client.GetAsync(_transactionsAPI);
                if (Res.IsSuccessStatusCode)
                {
                    var ObjResponse = Res.Content.ReadAsStringAsync().Result;
                    var serializedTransactions = JsonConvert.DeserializeObject<List<Models.TranItemResume>>(ObjResponse);
                    serializedTransactions = (from t in serializedTransactions
                                              select new Models.Transaction
                                              {
                                                  sku = t.sku,
                                                  amount = t.amount,
                                                  currency = t.currency
                                              })
                                                .Select((item, index) => new Models.TranItemResume
                                                {
                                                    index = index,
                                                    sku = item.sku,
                                                    currency = item.currency,
                                                    amount = item.amount
                                                })
                                                .ToList();
                    customResponse.Data = serializedTransactions;
                    customResponse.isSuccess = true;
                    var jsonBak = JsonConvert.SerializeObject(serializedTransactions, Formatting.Indented);
                    //write res to bak file
                    System.IO.File.WriteAllText(_bkpFilePath, jsonBak);
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
        public async Task<Models.CustomResponse<Models.TransactionResume>> GetTransactionBySku(string sku)
        {
            Models.CustomResponse<Models.TransactionResume> customResponse = new Models.CustomResponse<Models.TransactionResume>();
            Models.TransactionResume tranResume = new Models.TransactionResume();
            try
            {
                var client = new HttpClient();
                HttpResponseMessage Res = await client.GetAsync(_transactionsAPI);
                if (Res.IsSuccessStatusCode)
                {
                    var ObjResponse = Res.Content.ReadAsStringAsync().Result;
                    var serializedTransactions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.TranItemResume>>(ObjResponse);
                    customResponse = await CalculateTransactionResumeInEuro(serializedTransactions, sku);
                    customResponse.isSuccess = true;
                    return customResponse;
                }
                else
                {
                    return await GetTransactionBySkuBackup(sku);
                }
            }
            catch (Exception e)
            {
                customResponse.ErrorMessage = e.Message;
                customResponse.Data = null;
                return customResponse;
            }

        }
        public async Task<Models.CustomResponse<List<Models.TranItemResume>>> GetAllTransactionsBackup()
        {
            Models.CustomResponse<List<Models.TranItemResume>> customResponse = new Models.CustomResponse<List<Models.TranItemResume>>();
            try
            {
                string jsonFile = System.IO.File.ReadAllText(_bkpFilePath);
                var jsonTransactions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.TranItemResume>>(jsonFile);
                customResponse.Data = jsonTransactions;
                customResponse.Message = "This Data is returned from backup file since the API Endpoint was down";
                return customResponse;
            }
            catch (Exception e)
            {
                customResponse.ErrorMessage = e.Message;
                customResponse.Data = null;
                return customResponse;
            }
        }

        public async Task<Models.CustomResponse<Models.TransactionResume>> GetTransactionBySkuBackup(string sku)
        {
            Models.CustomResponse<Models.TransactionResume> customResponse = new Models.CustomResponse<Models.TransactionResume>();
            Models.TransactionResume tranResume = new Models.TransactionResume();
            try
            {
                string jsonFile = System.IO.File.ReadAllText(_bkpFilePath);
                var serializedTransactions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.TranItemResume>>(jsonFile);
                customResponse = await CalculateTransactionResumeInEuro(serializedTransactions, sku);

                customResponse.Message = "This Data is returned from backup file since the API Endpoint was down";
                return customResponse;
            }
            catch (Exception e)
            {
                customResponse.Message = "Error while trying to get Transaction Item.";
                customResponse.ErrorMessage = e.Message;
                return customResponse;
            }
        }

        private Models.TranItemResume GetConversionToEuro(Models.TranItemResume item, List<Models.Currency> allCurrencies)
        {
            double totalEuros = 0;
            var resCurrency = (from c in allCurrencies
                               where c.to == "EUR" && c.@from == item.currency
                               select new Models.Currency
                               {
                                   rate = c.rate
                               }).FirstOrDefault();
            if (resCurrency != null)
            {
                double result = Math.Round(item.amount * resCurrency.rate, 2, MidpointRounding.ToEven);
                totalEuros += result;
                item.amount = totalEuros;
                item.currency = "EUR";
            }
            else
            {
                var stepItem = GetStepConversionToEuro(item, allCurrencies);
                item.amount = stepItem.amount;
                item.currency = stepItem.currency;
            }
            return item;

        }
        private Models.Transaction GetStepConversionToEuro(Models.TranItemResume item, List<Models.Currency> allCurrencies)
        {
            var getStepCurrency = (from c in allCurrencies
                                   where c.@from == item.currency
                                   select new Models.Currency
                                   {
                                       rate = c.rate,
                                       @from = c.to
                                   }).FirstOrDefault();
            var stepConversion = Math.Round(item.amount * getStepCurrency.rate, 2, MidpointRounding.ToEven);

            var getStepTransaction = (from c in allCurrencies
                                      where c.@from == item.currency
                                      select new Models.Transaction
                                      {
                                          sku = item.sku,
                                          amount = stepConversion,
                                          currency = c.to
                                      }).FirstOrDefault();

            return getStepTransaction;
        }

        private async Task<Models.CustomResponse<Models.TransactionResume>> CalculateTransactionResumeInEuro(List<Models.TranItemResume> serializedTransactions, string sku)
        {
            Models.CustomResponse<Models.TransactionResume> customResponse = new Models.CustomResponse<Models.TransactionResume>();
            Models.TransactionResume tranResume = new Models.TransactionResume();
            var filteredTransaction = (from t in serializedTransactions
                                       where t.sku.ToLower() == sku.ToLower()
                                       select new Models.TranItemResume
                                       {
                                           index = t.index,
                                           sku = t.sku,
                                           amount = t.amount,
                                           currency = t.currency
                                       })
                                           .ToList();
            if (filteredTransaction.Any())
            {
                tranResume.transactionList = filteredTransaction;
                var totalEuros = filteredTransaction.AsEnumerable().Where(t => t.currency == "EUR").Sum(c => c.amount);
                tranResume.totalAmount = totalEuros;
                customResponse.Data = tranResume;
                var itemsNotEuro = (from t in filteredTransaction
                                    where t.currency != "EUR"
                                    select new Models.TranItemResume
                                    {
                                        index = t.index,
                                        sku = t.sku,
                                        currency = t.currency,
                                        amount = t.amount
                                    })
                                    .ToList();
                var allCurrencies = await _currencyService.GetAllCurrencies();
                if (allCurrencies.Data == null)
                {
                    var allCurrenciesbak = await _currencyService.GetAllCurrenciesBackup();
                    allCurrencies.Data = allCurrenciesbak.Data;
                }
                if (itemsNotEuro != null)
                {
                    Models.TranItemResume itemConvertedToEuros = new Models.TranItemResume();
                    foreach (var item in itemsNotEuro)
                    {
                        while (item.currency != "EUR")
                        {
                            itemConvertedToEuros = GetConversionToEuro(item, allCurrencies.Data);
                            item.currency = itemConvertedToEuros.currency;
                        }
                        inEuroList.Add(item);
                    };
                    var totalListConvertedToEuros = inEuroList.AsEnumerable().Where(t => t.currency == "EUR").Sum(c => c.amount);
                    var euroTransactionsGiven = (from te in filteredTransaction
                                                 where te.currency == "EUR"
                                                 select new Models.TranItemResume
                                                 {
                                                     index = te.index,
                                                     sku = te.sku,
                                                     currency = te.currency,
                                                     amount = te.amount
                                                 }).ToList();
                    var allProducts = new List<Models.TranItemResume>(euroTransactionsGiven.Count +
                                        inEuroList.Count);
                    allProducts.AddRange(euroTransactionsGiven);
                    allProducts.AddRange(inEuroList);
                    totalEuros += totalListConvertedToEuros;
                    tranResume.totalAmount = Math.Round(totalEuros, 2, MidpointRounding.ToEven);
                    var orderAllProducts = allProducts.OrderBy(x => x.index);
                    var finalResumeTranOrdered = (from tranItem in orderAllProducts
                                                  select new Models.TranItemResume
                                                  {
                                                      index = tranItem.index,
                                                      sku = tranItem.sku,
                                                      currency = tranItem.currency,
                                                      amount = tranItem.amount
                                                  }).ToList();

                    customResponse.Data.transactionList = finalResumeTranOrdered;
                }
                else
                {
                    customResponse.ErrorMessage = "Error while trying to get Transaction Item.";
                    return customResponse;
                }
            }
            else
            {
                return await GetTransactionBySkuBackup(sku);
            }
            customResponse.Message = "This Data is returned from backup file since the API Endpoint was down";
            return customResponse;
        }

    }
}
