namespace Jobs.Backend.Features.Shared.Interfaces;

public interface IApplicationError
{
    public string Code { get; }
    public string Message { get; init; }
}