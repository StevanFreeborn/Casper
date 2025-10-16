namespace Casper.Console.Tests.Integration.Interview;

public class InterviewAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public InterviewAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new InterviewAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task RunAsync_WhenCalled_ItShouldReturnNonNullInterviewAgentResponse()
  {
    var thread = _aut.GetNewThread();
    var response = await _aut.RunAsync("Explain the concept of dependency injection in software development.", cancellationToken: TestContext.Current.CancellationToken);
    var interviewResponse = JsonSerializer.Deserialize<InterviewAgentResponse>(response.Text);
    interviewResponse.Should().NotBeNull();
  }
}