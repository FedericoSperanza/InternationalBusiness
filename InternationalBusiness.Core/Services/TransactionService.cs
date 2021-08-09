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
        List<Models.Transaction> inEuroList = new List<Models.Transaction>();
        private readonly string _bkpFilePath;
        public TransactionService(IConfiguration config, CurrencyService currService)
        {
            _configuration = config;
            _transactionsAPI = _configuration["EndPoints:TransactionsAPI"];
            _currencyService = currService;
            _bkpFilePath = _configuration["BackupFilesPaths:TransactionsPath"];
        }

        public async Task<Models.CustomResponse<List<Models.Transaction>>> GetAllTransactions()
        {
            Models.CustomResponse<List<Models.Transaction>> customResponse = new Models.CustomResponse<List<Models.Transaction>>();
            try
            {
                var client = new HttpClient();
                HttpResponseMessage Res = await client.GetAsync(_transactionsAPI);
                if (Res.IsSuccessStatusCode)
                {
                    var ObjResponse = Res.Content.ReadAsStringAsync().Result;
                    var serializedTransactions = JsonConvert.DeserializeObject<List<Models.Transaction>>(ObjResponse);
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
                    var serializedTransactions = JsonConvert.DeserializeObject<List<Models.Transaction>>(ObjResponse);
                    var filteredTransaction = (from t in serializedTransactions
                                               where t.sku == sku
                                               select new Models.Transaction
                                               {
                                                   sku = t.sku,
                                                   amount = t.amount,
                                                   currency = t.currency
                                               }).ToList();
                    if (filteredTransaction.Any()) { 
                    tranResume.transactionList = filteredTransaction;
                    var totalEuros = filteredTransaction.AsEnumerable().Where(t => t.currency == "EUR").Sum(c => c.amount);
                    tranResume.totalAmount = totalEuros;
                    customResponse.Data = tranResume;
                    var itemsNotEuro = (from t in filteredTransaction
                                        where t.currency != "EUR"
                                        select new Models.Transaction
                                        {
                                            sku = t.sku,
                                            currency = t.currency,
                                            amount = t.amount
                                        }).ToList();

                    var allCurrencies = await _currencyService.GetAllCurrencies();
                    if (allCurrencies.Data == null)
                        {
                            var allCurrenciesbak = await _currencyService.GetAllCurrenciesBackup();
                            allCurrencies.Data = allCurrenciesbak.Data;
                        }
                    if (itemsNotEuro != null)
                    {
                        foreach (var item in itemsNotEuro)
                        {
                            var itemConvertedToEuros = GetConversionToEuro(item, allCurrencies.Data);
                            while (itemConvertedToEuros.currency != "EUR")
                            {
                                itemConvertedToEuros = GetConversionToEuro(item, allCurrencies.Data);

                            }
                            inEuroList.Add(itemConvertedToEuros);
                        };
                        var totalListConvertedToEuros = inEuroList.AsEnumerable().Where(t => t.currency == "EUR").Sum(c => c.amount);
                        var euroTransactionsGiven = (from te in filteredTransaction
                                                     where te.currency == "EUR"
                                                     select new Models.Transaction
                                                     {
                                                         sku = te.sku,
                                                         currency = te.currency,
                                                         amount = te.amount
                                                     }).ToList();
                        var allProducts = new List<Models.Transaction>(euroTransactionsGiven.Count +
                                                inEuroList.Count);
                        allProducts.AddRange(euroTransactionsGiven);
                        allProducts.AddRange(inEuroList);
                        totalEuros += totalListConvertedToEuros;
                        tranResume.totalAmount = totalEuros;
                        customResponse.Data.transactionList = allProducts;
                    }
                    customResponse.isSuccess = true;
                    return customResponse;
                    }
                    else
                    {
                        //sku not found
                        return await GetTransactionBySkuBackup(sku);
                    }
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
        public async Task<Models.CustomResponse<List<Models.Transaction>>> GetAllTransactionsBackup()
        {
            Models.CustomResponse<List<Models.Transaction>> customResponse = new Models.CustomResponse<List<Models.Transaction>>();
            try
            {
                string jsonFile = System.IO.File.ReadAllText(_bkpFilePath);
                var jsonTransactions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.Transaction>>(jsonFile);
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
                var serializedTransactions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.Transaction>>(jsonFile);
                var filteredTransaction = (from t in serializedTransactions
                                           where t.sku == sku
                                           select new Models.Transaction
                                           {
                                               sku = t.sku,
                                               amount = t.amount,
                                               currency = t.currency
                                           }).ToList();
                tranResume.transactionList = filteredTransaction;
                var totalEuros = filteredTransaction.AsEnumerable().Where(t => t.currency == "EUR").Sum(c => c.amount);
                tranResume.totalAmount = totalEuros;
                customResponse.Data = tranResume;
                var itemsNotEuro = (from t in filteredTransaction
                                    where t.currency != "EUR"
                                    select new Models.Transaction
                                    {
                                        sku = t.sku,
                                        currency = t.currency,
                                        amount = t.amount
                                    }).ToList();

                var allCurrencies = await _currencyService.GetAllCurrencies();
                if (allCurrencies.Data == null)
                {
                    var allCurrenciesbak = await _currencyService.GetAllCurrenciesBackup();
                    allCurrencies.Data = allCurrenciesbak.Data;
                }
                if (itemsNotEuro != null)
                {
                    foreach (var item in itemsNotEuro)
                    {
                        var itemConvertedToEuros = GetConversionToEuro(item, allCurrencies.Data);
                        while (itemConvertedToEuros.currency != "EUR")
                        {
                            itemConvertedToEuros = GetConversionToEuro(item, allCurrencies.Data);

                        }
                        inEuroList.Add(itemConvertedToEuros);
                    };
                    var totalListConvertedToEuros = inEuroList.AsEnumerable().Where(t => t.currency == "EUR").Sum(c => c.amount);
                    var euroTransactionsGiven = (from te in filteredTransaction
                                                 where te.currency == "EUR"
                                                 select new Models.Transaction
                                                 {
                                                     sku = te.sku,
                                                     currency = te.currency,
                                                     amount = te.amount
                                                 }).ToList();
                    var allProducts = new List<Models.Transaction>(euroTransactionsGiven.Count +
                                            inEuroList.Count);
                    allProducts.AddRange(euroTransactionsGiven);
                    allProducts.AddRange(inEuroList);
                    totalEuros += totalListConvertedToEuros;
                    tranResume.totalAmount = Math.Round(totalEuros, 2, MidpointRounding.ToEven); ;
                    
                    customResponse.Data.transactionList = allProducts;
                }
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

        private Models.Transaction GetConversionToEuro(Models.Transaction item, List<Models.Currency> allCurrencies)
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
        private Models.Transaction GetStepConversionToEuro(Models.Transaction item, List<Models.Currency> allCurrencies)
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

    }
}
