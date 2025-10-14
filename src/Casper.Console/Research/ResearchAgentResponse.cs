namespace Casper.Console.Research;

internal record ResearchAgentResponse(
  [property: JsonPropertyName("results")]
  string Results
);