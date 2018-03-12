using System;

namespace PsvJsonToPlex
{
    public class Result
    {
        public Result(bool isSuccess, string details, Exception ex)
        {
            (IsSuccess, Details, Exception) = (isSuccess, details, ex);
        }

        public bool IsSuccess { get; set; }
        public string Details { get; set; }
        public Exception Exception { get; set; }

        public static Result FromFailure(string details, Exception ex)
        {
            return new Result(false, details, ex);
        }

        public static Result FromSuccess(string details = null)
        {
            return new Result(true, details, null);
        }
    }
}