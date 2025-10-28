using System.IO.Abstractions;

namespace Casper.Console.Tests.Unit.Critique;

public class CriticTests : ExecutorTest
{
  private readonly Mock<IFileSystem> _mockFileSystem = new();
  private readonly Critic _sut;

  public CriticTests()
  {
    _sut = new(MockAgent.Object, _mockFileSystem.Object);
  }

  [Theory]
  [InlineData("null")]
  [InlineData("")]
  [InlineData("Invalid JSON")]
  public async Task HandleAsync_WhenCalledAndResponseCanNotBeDeserialized_ItShouldReturnAFailure(string agentOutput)
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

    MockContext.Verify(m => m.YieldOutputAsync(It.IsAny<string>(), TestContext.Current.CancellationToken));
  }

  [Fact]
  public async Task HandleAsync_WhenCalledAndHasFeedback_ItShouldReturnFailureWithFeedback()
  {
    var comment = TestDataFactory.Comment.Generate();

    var criticAgentResponse = TestDataFactory
      .CriticAgentResponse
      .Generate() with 
      { 
        HasFeedback = true,
        Comments = [comment],
      };

    var criticAgentResponseJson = JsonSerializer.Serialize(criticAgentResponse);

    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, criticAgentResponseJson)));

    var blogPost = TestDataFactory.BlogPost.Generate();
    var writerResult = new Success<BlogPost>(blogPost);
    
    var result = await _sut.HandleAsync(writerResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Failure>();

    var failure = result.As<Failure>();
    var feedback = failure.Exception.As<Feedback>();
    feedback.Comments[0].Should().BeEquivalentTo(comment);
    feedback.OriginalBlogPost.Should().BeEquivalentTo(blogPost);
  }

  [Fact]
  public async Task HandleAsync_WhenCalledAndDoesNotHaveFeedback_ItShouldReturnSuccessAndWriteBlogPostToDisk()
  {
    var mockPath = new Mock<IPath>();
    var mockFile = new Mock<IFile>();

    _mockFileSystem.SetupGet(m => m.Path).Returns(mockPath.Object);
    _mockFileSystem.SetupGet(m => m.File).Returns(mockFile.Object);

    var comment = TestDataFactory.Comment.Generate();

    var criticAgentResponse = TestDataFactory
      .CriticAgentResponse
      .Generate() with 
      { 
        HasFeedback = false,
        Comments = [],
      };

    var criticAgentResponseJson = JsonSerializer.Serialize(criticAgentResponse);

    MockAgent
      .Setup(
        static m => m.RunAsync(
          It.IsAny<IEnumerable<ChatMessage>>(),
          It.IsAny<AgentThread>(),
          It.IsAny<AgentRunOptions>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(new AgentRunResponse(new ChatMessage(ChatRole.Assistant, criticAgentResponseJson)));

    var blogPost = TestDataFactory.BlogPost.Generate();
    var writerResult = new Success<BlogPost>(blogPost);
    
    var result = await _sut.HandleAsync(writerResult, MockContext.Object, TestContext.Current.CancellationToken);

    result.Should().BeOfType<Success>();

    mockPath.Verify(m => m.Combine(It.IsAny<string>(), It.IsAny<string>()));
    mockFile.Verify(m => m.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), TestContext.Current.CancellationToken));
    MockContext.Verify(m => m.YieldOutputAsync(It.IsAny<string>(), TestContext.Current.CancellationToken));
  }
}