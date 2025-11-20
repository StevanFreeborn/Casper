namespace Casper.Console.Tests.Unit.Research;

public class ResearcherTests : ExecutorTest
{
  private readonly Researcher _sut;

  public ResearcherTests()
  {
    _sut = new(MockAgent.Object);
  }

  [Theory]
  [InlineData("null")]
  [InlineData("")]
  [InlineData("Invalid JSON")]
  public async Task HandleAsync_WhenCalledAndAgentResponseCantBeDeserialized_ItShouldReturnFailure(string agentOutput)
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

    var topicBrief = TestDataFactory.TopicBrief.Generate();
    var interviewerResult = new Success<TopicBrief>(topicBrief);

    var result = await _sut.HandleAsync(interviewerResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Failure>();
  }

  [Fact]
  public async Task HandleAsync_WhenCalledAndResponseContainsResearchBrief_ItShouldReturnAWritersBrief()
  {
    var researchBrief = TestDataFactory.ResearchBrief.Generate();
    var researchAgentResponse = TestDataFactory
      .ResearchAgentResponse
      .Generate() with { Brief = researchBrief };
    
    var researchAgentResponseJson = JsonSerializer.Serialize(researchAgentResponse);

    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, researchAgentResponseJson)));

    var topicBrief = TestDataFactory.TopicBrief.Generate();
    var interviewerResult = new Success<TopicBrief>(topicBrief);

    var result = await _sut.HandleAsync(interviewerResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Success<WriterBrief>>();

    var success = result.As<Success<WriterBrief>>();
    success.Value.TopicBrief.Should().BeEquivalentTo(topicBrief);
    success.Value.ResearchBrief.Should().BeEquivalentTo(researchBrief);
  }
}