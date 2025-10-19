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
)
{
  public override string ToString()
  {
    var sb = new StringBuilder();

    sb.AppendLine("[Subject]");
    sb.AppendLine(Subject);
    sb.AppendLine();

    sb.AppendLine("[Thesis]");
    sb.AppendLine(Thesis);
    sb.AppendLine();

    sb.AppendLine("[Research Points]");
    foreach (var point in ResearchPoints)
    {
      sb.AppendLine(CultureInfo.InvariantCulture, $"- {point.KeyPointTitle}");
      sb.AppendLine("  [Supporting Data]");
      foreach (var data in point.SupportingData)
      {
        sb.AppendLine(CultureInfo.InvariantCulture, $"    - {data.Finding} (Source: {data.Source})");
      }
      sb.AppendLine("  [Expert Opinions]");
      foreach (var opinion in point.ExpertOpinions)
      {
        sb.AppendLine(CultureInfo.InvariantCulture, $"    - {opinion.Finding} (Source: {opinion.Source})");
      }
      sb.AppendLine("  [Examples]");
      foreach (var example in point.Examples)
      {
        sb.AppendLine(CultureInfo.InvariantCulture, $"    - {example.Finding} (Source: {example.Source})");
      }
    }

    sb.AppendLine("[Additional Resources]");
    foreach (var resource in AdditionalResources)
    {
      sb.AppendLine(CultureInfo.InvariantCulture, $"- {resource.Description} (Source: {resource.Source})");
    }

    return sb.ToString();
  }

  public string ToJSON() => JsonSerializer.Serialize(this);

  public string ToMarkdown()
  {
    var sb = new StringBuilder();

    sb.AppendLine(CultureInfo.InvariantCulture, $"# {Subject}");
    sb.AppendLine();

    sb.AppendLine($"## Thesis");
    sb.AppendLine(Thesis);
    sb.AppendLine();

    sb.AppendLine($"## Research Points");
    foreach (var point in ResearchPoints)
    {
      sb.AppendLine(CultureInfo.InvariantCulture, $"### {point.KeyPointTitle}");
      sb.AppendLine();

      sb.AppendLine($"#### Supporting Data");
      foreach (var data in point.SupportingData)
      {
        sb.AppendLine(CultureInfo.InvariantCulture, $"- {data.Finding} (Source: {data.Source})");
      }
      sb.AppendLine();

      sb.AppendLine($"#### Expert Opinions");
      foreach (var opinion in point.ExpertOpinions)
      {
        sb.AppendLine(CultureInfo.InvariantCulture, $"- {opinion.Finding} (Source: {opinion.Source})");
      }
      sb.AppendLine();

      sb.AppendLine($"#### Examples");
      foreach (var example in point.Examples)
      {
        sb.AppendLine(CultureInfo.InvariantCulture, $"- {example.Finding} (Source: {example.Source})");
      }
    }
    sb.AppendLine();

    sb.AppendLine($"## Additional Resources");

    foreach (var resource in AdditionalResources)
    {
      sb.AppendLine(CultureInfo.InvariantCulture, $"- {resource.Description} (Source: {resource.Source})");
    }

    return sb.ToString();
  }
}

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