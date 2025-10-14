namespace Casper.Console.Tests.Integration.Interview;

public class InterviewAgentTests : IntegrationTest
{
  private readonly InterviewAgent _sut;

  public InterviewAgentTests(TestConfiguration config) : base(config)
  {
    _sut = new InterviewAgent(GeminiChatClient);
  }

  [Fact]
  public async Task Test()
  {
    var response = await _sut.RunAsync("Explain the concept of dependency injection in software development.", cancellationToken: TestContext.Current.CancellationToken);
    response.Should().NotBeNull();
  }
}