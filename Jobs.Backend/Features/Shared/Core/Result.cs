using Jobs.Backend.Features.Shared.Interfaces;

namespace Jobs.Backend.Features.Shared.Core;

public abstract class Result<T>
{
    private Result() { }

    public static Result<T> Success(T value) => new Ok(value);
    public static Result<T> Failure(IApplicationError error) => new Fail(error);

    public static implicit operator Result<T>(T value) => new Ok(value);

    public sealed class Ok : Result<T>
    {
        public T Value { get; }
        public Ok(T value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }
    }

    public sealed class Fail : Result<T>
    {
        public IApplicationError Error { get; }
        public Fail(IApplicationError error)
        {
            if (error is null) throw new ArgumentNullException(nameof(error));
            Error = error;
        }
    }
}