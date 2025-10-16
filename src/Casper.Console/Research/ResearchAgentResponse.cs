namespace Casper.Console.Research;

internal sealed record ResearchAgentResponse(
  [property: JsonPropertyName("brief")]
  ResearchBrief Brief
);