namespace VStore.Domain.Shared;

public class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error != null)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
    private static Result<TValue> Success<TValue>(TValue value) => new(value, true, null);
    protected static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    public static implicit operator Result(Error error) => Failure(error);

    protected static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error? error)
        : base(isSuccess, error) =>
        _value = value;

    public TValue? Value => IsSuccess
        ? _value!
        : default;

    public static Result<TValue> Success(TValue value) => new(value, true, null);
    public new static Result<TValue> Failure(Error error) => new(default, false, error);
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
}