using System;
using System.Collections.Generic;
using System.Linq;

namespace MCIApi.Application.Common
{
    public enum ServiceErrorType
    {
        None = 0,
        Validation = 1,
        Conflict = 2,
        NotFound = 3,
        Unexpected = 4,
        Unauthorized = 5
    }

    public record ServiceError(string Code, string Message);

    public class ServiceResult<T>
    {
        public bool Success { get; }
        public T? Data { get; }
        public ServiceErrorType ErrorType { get; }
        public string? ErrorCode { get; }
        public IReadOnlyCollection<ServiceError> Errors { get; }

        private ServiceResult(bool success, T? data, ServiceErrorType errorType, string? errorCode, IReadOnlyCollection<ServiceError>? errors)
        {
            Success = success;
            Data = data;
            ErrorType = errorType;
            ErrorCode = errorCode;
            Errors = errors ?? Array.Empty<ServiceError>();
        }

        public static ServiceResult<T> Ok(T data) => new(true, data, ServiceErrorType.None, null, null);

        public static ServiceResult<T> Fail(ServiceErrorType type, string errorCode, string? message = null)
            => new(false, default, type, errorCode, message is null ? null : new[] { new ServiceError(errorCode, message) });

        public static ServiceResult<T> Fail(ServiceErrorType type, IEnumerable<ServiceError> errors, string? errorCode = null)
            => new(false, default, type, errorCode, errors.ToArray());
    }

    public class ServiceResult
    {
        public bool Success { get; }
        public ServiceErrorType ErrorType { get; }
        public string? ErrorCode { get; }
        public IReadOnlyCollection<ServiceError> Errors { get; }

        private ServiceResult(bool success, ServiceErrorType errorType, string? errorCode, IReadOnlyCollection<ServiceError>? errors)
        {
            Success = success;
            ErrorType = errorType;
            ErrorCode = errorCode;
            Errors = errors ?? Array.Empty<ServiceError>();
        }

        public static ServiceResult Ok() => new(true, ServiceErrorType.None, null, null);

        public static ServiceResult Fail(ServiceErrorType type, string errorCode, string? message = null)
            => new(false, type, errorCode, message is null ? null : new[] { new ServiceError(errorCode, message) });

        public static ServiceResult Fail(ServiceErrorType type, IEnumerable<ServiceError> errors, string? errorCode = null)
            => new(false, type, errorCode, errors.ToArray());
    }
}


