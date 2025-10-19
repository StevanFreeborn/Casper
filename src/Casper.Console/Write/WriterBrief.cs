namespace Casper.Console.Write;

internal sealed record WriterBrief(
  [property: JsonPropertyName("topicBrief")]
  TopicBrief TopicBrief,
  [property: JsonPropertyName("researchBrief")]
  ResearchBrief ResearchBrief
);