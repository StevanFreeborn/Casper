namespace Casper.Console.Common;

internal class Success : Result
{
  public Success()
  {
    IsSuccess = true;
  }
}

internal sealed class Success<T>(T value) : Success()
{
  public T Value { get; } = value;
}