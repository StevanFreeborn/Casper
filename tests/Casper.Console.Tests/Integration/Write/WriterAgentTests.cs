using Casper.Console.Tests.Data;
using Casper.Console.Write;

namespace Casper.Console.Tests.Integration.Write;

public class WriterAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public WriterAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new WriterAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task RunAsync_WhenCalled_ItShouldReturnAResponse()
  {
    var researchBrief = await TestDataFactory.TestResearchBrief();
    var writerBrief = new WriterBrief(
      TestDataFactory.TestTopicBrief,
      researchBrief
    );
    var writerBriefMsg = writerBrief.ToString();
    var response = await _aut.RunAsync(writerBriefMsg, cancellationToken: TestContext.Current.CancellationToken);
    response.Should().NotBeNull();
  }

}