using System.Text.Json;

using Casper.Console.Research;
using Casper.Console.Tests.Integration.Infrastructure;

using GeminiDotnet.ContentGeneration;

namespace Casper.Console.Tests.Integration.Research;

public class ResearchAgentTests : IntegrationTest
{
  private readonly ResearchAgent _sut;
  private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

  public ResearchAgentTests(TestConfiguration config) : base(config)
  {
    _sut = new ResearchAgent(GeminiChatClient);
  }

  [Fact]
  public async Task Test()
  {
    var response = await _sut.RunAsync("Who won the euro 2024?", cancellationToken: TestContext.Current.CancellationToken);

    var json = JsonSerializer.Serialize((response.RawRepresentation as GenerateContentResponse)?.Candidates[0].GroundingMetadata, _jsonOptions);

    TestContext.Current.SendDiagnosticMessage(json);
  }
}