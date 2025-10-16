[assembly: CaptureConsole]

namespace Casper.Console.Tests.Integration.Research;

public class ResearchAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public ResearchAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new ResearchAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task Test()
  {
    var response = await _aut.RunAsync(_topicBrief.ToString(), cancellationToken: TestContext.Current.CancellationToken);
    response.Should().NotBeNull();
  }

  private readonly TopicBrief _topicBrief = new(
    "Why Your TypeScript Skills Make C# Your Next Superpower",
    "An overview of why C# is an approachable and powerful next step for experienced TypeScript developers.",
    "Due to syntactic similarities and shared paradigms, TypeScript developers can quickly become productive in C# and gain access to a mature, high-performance ecosystem for building applications beyond the browser.",
    "Mid-to-senior level TypeScript developers who are interested in backend development or are looking to expand their skillset beyond the Node.js ecosystem.",
    [
      "Highlight the familiar syntax and language features like classes, interfaces, generics, and async/await.",
      "Showcase the power of the .NET ecosystem, particularly ASP.NET Core for APIs and Entity Framework for database access.",
      "Emphasize the world-class developer experience with tools like Visual Studio and JetBrains Rider."
    ],
    "Informative, pragmatic, and encouraging, framed as a peer-to-peer recommendation.",
    "Challenge the reader to scaffold a new '.NET Minimal API' and build a simple endpoint in under 30 minutes."
  );
}