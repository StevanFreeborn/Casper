namespace Casper.Console.Research;

internal record ResearchAgentResponse(
  [property: JsonPropertyName("brief")]
  ResearchBrief Brief
);