namespace Casper.Console.Tests.Unit.Interview;

public class InterviewerTests : ExecutorTest
{
  private readonly Interviewer _sut;

  public InterviewerTests()
  {
    _sut = new(MockAgent.Object);
  }

  [Theory]
  [InlineData("null")]
  [InlineData("")]
  [InlineData("Invalid JSON")]
  public async Task HandleAsync_WhenCalledAndResponseCantBeDeserialized_ItShouldReturnAFailure(string agentOutput)
  {
    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, agentOutput)));

    var message = new ChatMessage(ChatRole.User, "Test message");

    var result = await _sut.HandleAsync(message, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Failure>();
  }

  [Fact]
  public async Task HandleAsync_WhenCalledAndAgentNeedsMoreInfo_ItShouldReturnAFailureWithAQuestion()
  {
    var interviewerResponse = TestDataFactory
      .InterviewAgentResponse
      .Generate() with { NeedMoreInfo = true };

    var interviewerResponseJson = JsonSerializer.Serialize(interviewerResponse);

    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, interviewerResponseJson)));

    var message = new ChatMessage(ChatRole.User, "Test message");

    var result = await _sut.HandleAsync(message, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Failure>();

    var failure = result.As<Failure>();
    failure.Exception.Should().BeOfType<Question>();

    MockContext.Verify(m => m.SendMessageAsync(failure.Exception, cancellationToken: TestContext.Current.CancellationToken));
  }

  [Fact]
  public async Task HandleAsync_WhenCalledAndInterviewerIsSatisifed_ItShouldReturnTopicBrief()
  {
    var topicBrief = TestDataFactory.TopicBrief.Generate();

    var interviewerResponse = TestDataFactory
      .InterviewAgentResponse
      .Generate() with 
      { 
        NeedMoreInfo = false,
        Topic = topicBrief,
      };

    var interviewerResponseJson = JsonSerializer.Serialize(interviewerResponse);

    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, interviewerResponseJson)));

    var message = new ChatMessage(ChatRole.User, "Test message");

    var result = await _sut.HandleAsync(message, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Success<TopicBrief>>();

    var success = result.As<Success<TopicBrief>>();
    success.Value.Should().BeEquivalentTo(topicBrief);
  }
}