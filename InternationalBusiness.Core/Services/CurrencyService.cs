﻿using System;
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
    public class CurrencyService : ICurrencyService
    {
        private readonly string _currenciesApi;
        private readonly IConfiguration _configuration;
        
        public CurrencyService(IConfiguration config)
        {
            _configuration = config;
            _currenciesApi = _configuration["EndPoints:CurrenciesAPI"];
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
        public async Task<Models.CustomResponse<List<Models.Currency>>> GetAllCurrenciesBackup(string jsonFile)
        {
            Models.CustomResponse<List<Models.Currency>> response = new Models.CustomResponse<List<Models.Currency>>();
            try
            {       
                var jsonCurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.Currency>>(jsonFile);
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
        public async Task<Models.CustomResponse<Models.Currency>> GetCurrencyByTypeBackup(string jsonFile, string currencyType)
        {
            Models.CustomResponse<Models.Currency> response = new Models.CustomResponse<Models.Currency>();
            var jsonCurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.Currency>>(jsonFile);
            var filteredCurrency = (from c in jsonCurrencies
                                    where c.@from == currencyType
                                    select new { c }).FirstOrDefault();

            response.Data = filteredCurrency.c;
            response.Message = "This Data is returned from backup file since the API Endpoint was down";
            return response;
        }
    }
}