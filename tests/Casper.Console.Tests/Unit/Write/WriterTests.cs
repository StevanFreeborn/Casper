namespace Casper.Console.Tests.Unit.Write;

public class WriterTests : ExecutorTest
{
  private readonly Writer _sut;

  public WriterTests()
  {
    _sut = new(MockAgent.Object);
  }

  [Theory]
  [InlineData("null")]
  [InlineData("")]
  [InlineData("Invalid JSON")]
  public async Task HandleAsync_WhenCalledWithWriterBriefAndResponseCanNotBeDeserialized_ItShouldReturnFailure(string agentOutput)
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

    var writerBrief = TestDataFactory.WriterBrief.Generate();
    var researcherResult = new Success<WriterBrief>(writerBrief);
    
    var result = await _sut.HandleAsync(researcherResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Failure>();
  }

  [Fact]
  public async Task HandleAsync_WhenCalledWithWriterBriefAndResponseCanBeDeserialized_ItShouldReturnABlogPost()
  {
    var blogPost = TestDataFactory.BlogPost.Generate();
    var writerAgentResponse = TestDataFactory
      .WriterAgentResponse
      .Generate() with { Post = blogPost };

    var writerAgentResponseJson = JsonSerializer.Serialize(writerAgentResponse);

    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, writerAgentResponseJson)));

    var writerBrief = TestDataFactory.WriterBrief.Generate();
    var researcherResult = new Success<WriterBrief>(writerBrief);
    
    var result = await _sut.HandleAsync(researcherResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Success<BlogPost>>();
    
    var success = result.As<Success<BlogPost>>();
    success.Value.Should().BeEquivalentTo(blogPost);
  }

  [Theory]
  [InlineData("null")]
  [InlineData("")]
  [InlineData("Invalid JSON")]
  public async Task HandleAsync_WhenCalledWithFeedbackAndResponseCanNotBeDeserialized_ItShouldReturnFailure(string agentOutput)
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

    var feedback = TestDataFactory.Feedback.Generate();
    var feedbackResult = new Failure<Feedback>(feedback);
    
    var result = await _sut.HandleAsync(feedbackResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Failure>();
  }

  [Fact]
  public async Task HandleAsync_WhenCalledWithFeedbackAndResponseCanBeDeserialized_ItShouldReturnABlogPost()
  {
    var blogPost = TestDataFactory.BlogPost.Generate();
    var writerAgentResponse = TestDataFactory
      .WriterAgentResponse
      .Generate() with { Post = blogPost };

    var writerAgentResponseJson = JsonSerializer.Serialize(writerAgentResponse);

    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, writerAgentResponseJson)));

    var feedback = TestDataFactory.Feedback.Generate();
    var feedbackResult = new Failure<Feedback>(feedback);

    var result = await _sut.HandleAsync(feedbackResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Success<BlogPost>>();
    
    var success = result.As<Success<BlogPost>>();
    success.Value.Should().BeEquivalentTo(blogPost);
  }
}