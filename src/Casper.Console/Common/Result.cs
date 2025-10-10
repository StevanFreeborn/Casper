namespace Casper.Console.Common;

internal class Result
{
  private readonly Exception _error;

  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public Exception Error
  {
    get
    {
      if (IsSuccess)
      {
        throw new InvalidOperationException("Cannot access the error of a successful result.");
      }

      return _error;
    }
  }

  protected Result(bool isSuccess, Exception? error)
  {
    if (isSuccess && error is not null)
    {
      throw new InvalidOperationException($"A successful result cannot have an {nameof(error)}");
    }

    IsSuccess = isSuccess;
    _error = error ?? new Exception();
  }

  public static Result Success()
  {
    return new(true, null);
  }

  public static Result Failure(string errorMessage)
  {
    return new(false, new Exception(errorMessage));
  }

  public static Result Failure(Exception error)
  {
    return new(false, error);
  }

  public static Result<T> Success<T>(T value)
  {
    return new Result<T>(value, true, null);
  }

  public static Result<T> Failure<T>(Exception exception)
  {
    return new Result<T>(default!, false, exception);
  }
}

internal class Result<T> : Result
{
  private readonly T _value;

  public T Value
  {
    get
    {
      if (IsSuccess is false)
      {
        throw new InvalidOperationException("Cannot access the value of a failed result. Check {nameof(Error)} property for more details.");
      }

      return _value;
    }
  }

  protected internal Result(T value, bool isSuccess, Exception? exception) : base(isSuccess, exception)
  {
    _value = value;
  }
}