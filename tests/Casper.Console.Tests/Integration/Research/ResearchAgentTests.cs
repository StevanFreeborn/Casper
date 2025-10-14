
namespace Casper.Console.Tests.Integration.Research;

public class ResearchAgentTests : IntegrationTest
{
  private readonly ResearchAgent _sut;

  public ResearchAgentTests(TestConfiguration config) : base(config)
  {
    _sut = new ResearchAgent(GeminiChatClient);
  }

  [Fact]
  public async Task Test()
  {
    var response = await _sut.RunAsync("Why TypeScript is less performant than C#", cancellationToken: TestContext.Current.CancellationToken);
    response.Should().NotBeNull();
  }
}