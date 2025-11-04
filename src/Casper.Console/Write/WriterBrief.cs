namespace Casper.Console.Write;

internal sealed record WriterBrief(
  [property: JsonPropertyName("topicBrief")]
  TopicBrief TopicBrief,
  [property: JsonPropertyName("researchBrief")]
  ResearchBrief ResearchBrief
)
{
  public override string ToString()
  {
    var sb = new StringBuilder();
    sb.AppendLine(TopicBrief.ToString());
    sb.AppendLine();
    sb.AppendLine(ResearchBrief.ToString());
    return sb.ToString();
  }
}