namespace VStore.Domain.Shared;

public class ValidationError(Error[] errors)
    : Error("Error.Validation", "One or more validation failures have occurred.")
{
    public Error[]? Errors { get; set; } = errors;
}