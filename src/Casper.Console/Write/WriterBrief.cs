namespace Casper.Console.Write;

internal sealed record WriterBrief(
  [property: JsonPropertyName("topicBrief")]
  TopicBrief TopicBrief,
  [property: JsonPropertyName("researchBrief")]
  ResearchBrief ResearchBrief
)
{
  public override string ToString() => TopicBrief.ToString() + '\n' + ResearchBrief.ToString();
}