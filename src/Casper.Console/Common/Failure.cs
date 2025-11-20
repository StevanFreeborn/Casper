namespace Casper.Console.Common;

internal class Failure : Result
{
  public Exception Exception { get; }

  public Failure(Exception exception)
  {
    IsSuccess = false;
    Exception = exception;
  }

  public Failure(string message)
  {
    IsSuccess = false;
    Exception = new Exception(message);
  }

  public override string ToString() => Exception.ToString();
}

internal sealed class Failure<T> : Failure where T : Exception
{
  public new T Exception { get; }

  public Failure(T exception) : base(exception)
  {
    Exception = exception;
  }
}