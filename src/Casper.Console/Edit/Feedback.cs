namespace Casper.Console.Edit;

internal sealed class Feedback : Exception
{
  public string[] Comments { get; init; } = [];

  public Feedback()
  {
  }

  public Feedback(string[] comments)
  {
    Comments = comments;
  }

  public Feedback(string message) : base(message)
  {
  }

  public Feedback(string message, Exception innerException) : base(message, innerException)
  {
  }
}