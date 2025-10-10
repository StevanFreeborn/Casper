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
}
