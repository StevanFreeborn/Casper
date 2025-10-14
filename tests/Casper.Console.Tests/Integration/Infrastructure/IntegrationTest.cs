
namespace Casper.Console.Tests.Integration.Infrastructure;

public abstract class IntegrationTest : IClassFixture<TestConfiguration>
{
  protected IChatClient GeminiChatClient { get; }

  protected IntegrationTest(TestConfiguration config)
  {
    ArgumentNullException.ThrowIfNull(config);

    GeminiChatClient = new GeminiChatClient(config.Options);
  }
}