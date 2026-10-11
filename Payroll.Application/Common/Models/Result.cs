namespace Payroll.Application.Common.Models
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Forbidden,
        BusinessRule
    }
    public sealed record Error(string Code, string Message, ErrorType Type)
    {
        public static Error NotFound(string entity, object key) =>
            new($"{entity}.notFound", $"{entity} '{key}' was not found.", ErrorType.NotFound);

        public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
        public static Error Rule(string code, string message) => new(code, message, ErrorType.BusinessRule);
        public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);
        public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);
        public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    }
    public class Result
    {
        protected Result(Error? error) => Error = error;

        public Error? Error { get; }
        public bool IsSuccess => Error is null;

        public static Result Success() => new(null);
        public static Result Failure(Error error) => new(error);
        public static Result<T> Success<T>(T value) => new(value, null);

        public static implicit operator Result(Error error) => Failure(error);
    }
    public sealed class Result<T> : Result
    {
        internal Result(T? value, Error? error) : base(error) => Value = value;

        public T? Value { get; }

        public static implicit operator Result<T>(T value) => new(value, null);
        public static implicit operator Result<T>(Error error) => new(default, error);
    }
}
