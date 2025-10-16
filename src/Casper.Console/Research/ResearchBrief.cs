namespace Casper.Console.Research;

internal sealed record ResearchBrief(
  [property: JsonPropertyName("subject")]
  string Subject,
  [property: JsonPropertyName("thesis")]
  string Thesis,
  [property: JsonPropertyName("researchPoints")]
  ResearchPoint[] ResearchPoints,
  [property: JsonPropertyName("additionalResources")]
  AdditionalResource[] AdditionalResources
);

internal sealed record ResearchPoint(
  [property: JsonPropertyName("keyPointTitle")]
  string KeyPointTitle,
  [property: JsonPropertyName("supportingData")]
  Citation[] SupportingData,
  [property: JsonPropertyName("expertOpinions")]
  Citation[] ExpertOpinions,
  [property: JsonPropertyName("examples")]
  Citation[] Examples
);

internal sealed record Citation(
  [property: JsonPropertyName("finding")]
  string Finding,
  [property: JsonPropertyName("source")]
  string Source
);

internal sealed record AdditionalResource(
  [property: JsonPropertyName("description")]
  string Description,
  [property: JsonPropertyName("source")]
  string Source
);