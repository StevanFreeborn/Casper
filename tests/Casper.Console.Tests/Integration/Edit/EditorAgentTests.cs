using Casper.Console.Edit;
using Casper.Console.Tests.Data;

namespace Casper.Console.Tests.Integration.Edit;

public class EditorAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public EditorAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new EditorAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task RunAsync_WhenCalled_ItShouldReturnAResponse()
  {
    var blogPostMsg = await TestDataFactory.TestBlogPost();

    var response = await _aut.RunAsync(blogPostMsg.ToString(), cancellationToken: TestContext.Current.CancellationToken);

    response.Should().NotBeNull();
  }
}