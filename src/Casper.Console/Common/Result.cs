namespace Casper.Console.Common;

internal abstract class Result
{
  public bool IsSuccess { get; protected set; }
  public bool IsFailure => !IsSuccess;

  public static Result Ok<T>(T value) => new Success<T>(value);
  public static Result Ok() => new Success();
  public static Result Fail(Exception exception) => new Failure(exception);
  public static Result Fail<T>(T exception) where T : Exception => new Failure<T>(exception);
  public static Result Fail(string message) => new Failure(message);
}