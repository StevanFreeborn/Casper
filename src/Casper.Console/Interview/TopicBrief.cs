namespace Casper.Console.Interview;

internal record TopicBrief(
  [property: JsonPropertyName("workingTitleSuggestion")]
  string WorkingTitleSuggestion,
  [property: JsonPropertyName("topic")]
  string Topic,
  [property: JsonPropertyName("coreThesis")]
  string CoreThesis,
  [property: JsonPropertyName("targetAudience")]
  string TargetAudience,
  [property: JsonPropertyName("keyPoints")]
  string[] KeyPoints,
  [property: JsonPropertyName("desiredToneAndVoice")]
  string DesiredToneAndVoice,
  [property: JsonPropertyName("callToAction")]
  string CallToAction
)
{
  public override string ToString()
  {
    var sb = new StringBuilder();

    sb.AppendLine("Topic Brief:");
    sb.AppendLine("-------------");

    sb.AppendLine("[Working Title Suggestion]");
    sb.AppendLine(WorkingTitleSuggestion);
    sb.AppendLine();

    sb.AppendLine("[Topic]");
    sb.AppendLine(Topic);
    sb.AppendLine();

    sb.AppendLine("[Core Thesis]");
    sb.AppendLine(CoreThesis);
    sb.AppendLine();

    sb.AppendLine("[Target Audience]");
    sb.AppendLine(TargetAudience);
    sb.AppendLine();

    sb.AppendLine("[Key Points]");
    foreach (var point in KeyPoints)
    {
      sb.AppendLine(CultureInfo.InvariantCulture, $"- {point}");
    }
    sb.AppendLine();

    sb.AppendLine("[Desired Tone and Voice]");
    sb.AppendLine(DesiredToneAndVoice);
    sb.AppendLine();

    sb.AppendLine("[Call to Action]");
    sb.AppendLine(CallToAction);

    return sb.ToString();
  }

  public string ToJson() => JsonSerializer.Serialize(this);

  public string ToMarkdown()
  {
    var sb = new StringBuilder();

    sb.AppendLine("# Topic Brief");

    sb.AppendLine("## Working Title Suggestion");
    sb.AppendLine(WorkingTitleSuggestion);
    sb.AppendLine();

    sb.AppendLine("## Topic");
    sb.AppendLine(Topic);
    sb.AppendLine();

    sb.AppendLine("## Core Thesis");
    sb.AppendLine(CoreThesis);
    sb.AppendLine();

    sb.AppendLine("## Target Audience");
    sb.AppendLine(TargetAudience);
    sb.AppendLine();

    sb.AppendLine("## Key Points");
    foreach (var point in KeyPoints)
    {
      sb.AppendLine(CultureInfo.InvariantCulture, $"- {point}");
    }
    sb.AppendLine();

    sb.AppendLine("## Desired Tone and Voice");
    sb.AppendLine(DesiredToneAndVoice);
    sb.AppendLine();

    sb.AppendLine("## Call to Action");
    sb.AppendLine(CallToAction);

    return sb.ToString();
  }
}