
namespace Casper.Console.Tests.Integration.Critique;

public class CriticAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public CriticAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new CriticAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task RunAsync_WhenCalled_ItShouldReturnAResponse()
  {
    var blogPost = await TestDataFactory.TestBlogPost.Value;
    var blogPostMsg = blogPost.ToMarkdown();

    var response = await _aut.RunAsync(blogPostMsg, cancellationToken: TestContext.Current.CancellationToken);

    response.Should().NotBeNull();
  }
}