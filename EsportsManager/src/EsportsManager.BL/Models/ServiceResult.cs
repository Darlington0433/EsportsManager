using System.Collections.Generic;
using System.Linq;

namespace EsportsManager.BL.Interfaces
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ServiceResult Success()
        {
            return new ServiceResult { IsSuccess = true };
        }

        public static ServiceResult Failure(string errorMessage)
        {
            return new ServiceResult { IsSuccess = false, ErrorMessage = errorMessage };
        }

        public static ServiceResult Failure(List<string> errors)
        {
            return new ServiceResult { IsSuccess = false, Errors = errors ?? new List<string>() };
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T> { IsSuccess = true, Data = data };
        }

        public static new ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T> { IsSuccess = false, ErrorMessage = errorMessage };
        }

        public static new ServiceResult<T> Failure(List<string> errors)
        {
            return new ServiceResult<T> { IsSuccess = false, Errors = errors ?? new List<string>() };
        }
    }
}
