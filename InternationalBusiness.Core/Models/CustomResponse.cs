using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace InternationalBusiness.Core.Models
{
    public class CustomResponse<T> : CustomResponse
    {
        public T Data { get; set; }

        public static CustomResponse<T> CreateSuccessResponse(string successMessage = "")
        {
            return new CustomResponse<T> { Message = successMessage, isSuccess = false };
        }

        public static CustomResponse<T> CreateSuccessResponse(T data, string successMessage = "", string confirmation = "")
        {
            return new CustomResponse<T>
            {
                Data = data,
                Message = successMessage,
                isSuccess = true,
                Confirmation = confirmation
            };
        }

    }
    public class CustomResponse
    {
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string Confirmation { get; set; }
        public bool isSuccess { get; set; }

        public static CustomResponse CreateErrorResponse(string errorMessage)
        {
            return new CustomResponse { ErrorMessage = errorMessage, isSuccess = false };
        }
        public static CustomResponse CreateSuccessResponse(string successMessage = "", string confirmation = "")
        {
            return new CustomResponse { Message = confirmation, Confirmation = confirmation };
        }
    }
}
