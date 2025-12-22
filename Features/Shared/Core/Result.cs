using AuthNetExample.Features.Shared.Interfaces;

namespace AuthNetExample.Features.Shared.Core;

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public IApplicationError? Error { get; init; }
}

public static class Result
{
    public static Result<T> Success<T>(T value) => new Result<T>
    {
        IsSuccess = true,
        Value = value,
        Error = null
    };

    public static Result<T> Failure<T>(IApplicationError error) => new Result<T>
    {
        IsSuccess = false,
        Value = default,
        Error = error
    };
}