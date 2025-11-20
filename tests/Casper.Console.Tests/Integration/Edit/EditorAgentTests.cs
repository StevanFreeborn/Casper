namespace Casper.Console.Tests.Integration.Edit;

public class EditorAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public EditorAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new EditorAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task RunAsync_WhenCalledAndGivenUneditedBlogPost_ItShouldReturnAResponse()
  {
    var blogPostMsg = await TestDataFactory.TestBlogPost.Value;

    var response = await _aut.RunAsync(blogPostMsg.ToString(), cancellationToken: TestContext.Current.CancellationToken);

    response.Should().NotBeNull();
  }

  [Fact]
  public async Task RunAsync_WhenCalledAndGivenEditedBlogPost_ItShouldReturnAResponse()
  {
    var editedBlogPostMsg = await TestDataFactory.TestRevisedBlogPost.Value;

    var response = await _aut.RunAsync(editedBlogPostMsg.ToString(), cancellationToken: TestContext.Current.CancellationToken);

    response.Should().NotBeNull();
  }
}