namespace Casper.Console.Tests.Unit.Edit;

public class EditorTest : ExecutorTest
{
  private readonly Editor _sut;

  public EditorTest()
  {
    _sut = new(MockAgent.Object, MockConsole.Object);
  }

  [Theory]
  [InlineData("null")]
  [InlineData("")]
  [InlineData("Invalid JSON")]
  public async Task HandleAsync_WhenCalledAndResponseCanNotBeDeserialized_ItShouldReturnFailures(string agentOutput)
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

    var blogPost = TestDataFactory.BlogPost.Generate();
    var writerResult = new Success<BlogPost>(blogPost);
    
    var result = await _sut.HandleAsync(writerResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Failure>();
  }

  [Fact]
  public async Task HandleAsync_WhenCalledAndThereIsFeedback_ItShouldShouldReturnFeedback()
  {
    var comment = TestDataFactory.Comment.Generate();

    var editorAgentResponse = TestDataFactory
      .EditorAgentResponse
      .Generate() with 
      { 
        HasFeedback = true,
        Comments = [comment],
      };

    var editorAgentResponseJson = JsonSerializer.Serialize(editorAgentResponse);

    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, editorAgentResponseJson)));

    var blogPost = TestDataFactory.BlogPost.Generate();
    var writerResult = new Success<BlogPost>(blogPost);
    
    var result = await _sut.HandleAsync(writerResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Failure>();

    var failure = result.As<Failure>();
    var feedback = failure.Exception.As<Feedback>();
    feedback.Comments[0].Should().BeEquivalentTo(comment);
    feedback.OriginalBlogPost.Should().BeEquivalentTo(blogPost);
  }
}