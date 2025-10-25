namespace Casper.Console.Tests.Unit.Research;

public class ResearcherTests : ExecutorTest
{
  private readonly Researcher _sut;

  public ResearcherTests()
  {
    _sut = new(MockAgent.Object);
  }
}