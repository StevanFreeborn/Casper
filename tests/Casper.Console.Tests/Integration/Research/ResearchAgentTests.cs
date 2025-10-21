using Casper.Console.Tests.Data;

[assembly: CaptureConsole]

namespace Casper.Console.Tests.Integration.Research;

public class ResearchAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public ResearchAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new ResearchAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task RunAsync_WhenCalled_ItShouldReturnAResponse()
  {
    var topicBriefMsg = TestDataFactory.TestTopicBrief.ToString();
    var response = await _aut.RunAsync(topicBriefMsg, cancellationToken: TestContext.Current.CancellationToken);
    response.Should().NotBeNull();
  }
}