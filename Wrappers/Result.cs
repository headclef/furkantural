using System.Collections.Generic;

namespace furkantural.Wrappers
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static Result Ok(string message = "")
        {
            return new Result { Success = true, Message = message };
        }

        public static Result Fail(string error)
        {
            return new Result { Success = false, Errors = new List<string> { error } };
        }

        public static Result Fail(List<string> errors)
        {
            return new Result { Success = false, Errors = errors };
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public static Result<T> Ok(T data, string message = "")
        {
            return new Result<T> { Success = true, Data = data, Message = message };
        }

        public new static Result<T> Fail(string error)
        {
            return new Result<T> { Success = false, Errors = new List<string> { error } };
        }

        public new static Result<T> Fail(List<string> errors)
        {
            return new Result<T> { Success = false, Errors = errors };
        }
    }
}
