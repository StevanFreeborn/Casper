namespace Casper.Console.Tests.Integration.Interview;

public record InterviewTestCase(
  string Name
)
{
  public override string ToString() => Name;
}