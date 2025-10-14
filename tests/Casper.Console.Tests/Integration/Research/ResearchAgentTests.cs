using Casper.Console.Research;
using Casper.Console.Tests.Integration.Infrastructure;

using GeminiDotnet.ContentGeneration;

using Microsoft.Extensions.AI;

[assembly: CaptureConsole]

namespace Casper.Console.Tests.Integration.Research;

public class ResearchAgentTests : IntegrationTest
{
  private readonly ResearchAgent _sut;

  public ResearchAgentTests(TestConfiguration config) : base(config)
  {
    _sut = new ResearchAgent(GeminiChatClient);
  }

  [Fact]
  public async Task Test()
  {
    var response = await _sut.RunAsync("Why TypeScript is less performant than C#", cancellationToken: TestContext.Current.CancellationToken);
    var originalChatResponse = response.RawRepresentation as ChatResponse;
    var originalGeminiResponse = originalChatResponse?.RawRepresentation as GenerateContentResponse;
    var metadata = originalGeminiResponse?.Candidates[0].GroundingMetadata;

    if (metadata is null || metadata.GroundingSupports is null)
    {
      throw new InvalidOperationException("No grounding metadata found");
    }

    foreach (var support in metadata.GroundingSupports)
    {
      TestContext.Current.SendDiagnosticMessage(support.Segment.Text);
    }
  }
}