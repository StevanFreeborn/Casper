namespace Casper.Console.Common;

internal static class ResultExtensions
{
  public static void Match(this Result result, Action<Success> onSuccess, Action<Failure> onFailure)
  {
    switch (result)
    {
      case Success success:
        onSuccess(success);
        break;
      case Failure failure:
        onFailure(failure);
        break;
      default:
        throw new InvalidOperationException("Unknown Result type");
    }
  }

  public static void Match<T>(this Result result, Action<T> onSuccess, Action<Failure> onFailure) where T : Success
  {
    switch (result)
    {
      case T success:
        onSuccess(success);
        break;
      case Failure failure:
        onFailure(failure);
        break;
      default:
        throw new InvalidOperationException("Unknown Result type");
    }
  }

  public static TResult Match<TResult>(this Result result, Func<Success, TResult> onSuccess, Func<Failure, TResult> onFailure)
  {
    return result switch
    {
      Success success => onSuccess(success),
      Failure failure => onFailure(failure),
      _ => throw new InvalidOperationException("Unknown Result type")
    };
  }

  public static TResult Match<T, TResult>(this Result result, Func<T, TResult> onSuccess, Func<Failure, TResult> onFailure) where T : Success
  {
    return result switch
    {
      T success => onSuccess(success),
      Failure failure => onFailure(failure),
      _ => throw new InvalidOperationException("Unknown Result type")
    };
  }
}