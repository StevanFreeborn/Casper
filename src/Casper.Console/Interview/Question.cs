namespace Casper.Console.Interview;

internal class Question : Exception
{
  public Question() : base("Can you tell me more about that?")
  {
  }

  public Question(string message) : base(message)
  {
  }

  public Question(string message, Exception innerException) : base(message, innerException)
  {
  }
}