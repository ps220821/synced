using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced.Core.Results
{
    public record OperationResult
    {
        public bool Succeeded { get; init; }
        public string? Message { get; init; }
        public string? ErrorCode { get; init; } // For client-side handling

        public static OperationResult Success()
            => new() { Succeeded = true };

        public static OperationResult Failure(string message, string? errorCode = null)
            => new() { Succeeded = false, Message = message, ErrorCode = errorCode };
    }

    public record OperationResult<T> : OperationResult
    {
        public T? Data { get; init; }

        public static OperationResult<T> Success(T data)
            => new() { Succeeded = true, Data = data };

        public static OperationResult<T> Failure(string message, string? errorCode = null)
            => new() { Succeeded = false, Message = message, ErrorCode = errorCode };
    }
}
