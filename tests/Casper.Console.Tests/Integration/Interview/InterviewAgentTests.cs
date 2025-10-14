namespace Casper.Console.Tests.Integration.Interview;

public class InterviewAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public InterviewAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new InterviewAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task Test()
  {
    var response = await _aut.RunAsync("Explain the concept of dependency injection in software development.", cancellationToken: TestContext.Current.CancellationToken);
    response.Should().NotBeNull();
  }
}