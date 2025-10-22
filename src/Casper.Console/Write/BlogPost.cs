namespace Casper.Console.Write;

internal sealed record BlogPost(
  [property: JsonPropertyName("title")]
  string Title,
  [property: JsonPropertyName("content")]
  string Content,
  [property: JsonPropertyName("openGraphDescription")]
  string OpenGraphDescription,
  [property: JsonPropertyName("slug")]
  string Slug
)
{
  public override string ToString()
  {
    var sb = new StringBuilder();

    sb.AppendLine("[Title]");
    sb.AppendLine(Title);
    sb.AppendLine();

    sb.AppendLine("[Content]");
    sb.AppendLine(Content);
    sb.AppendLine();

    sb.AppendLine("[OpenGraphDescription]");
    sb.AppendLine(OpenGraphDescription);
    sb.AppendLine();

    sb.AppendLine("[Slug]");
    sb.AppendLine(Slug);
    sb.AppendLine();

    return sb.ToString();
  }

  public string ToJson() => JsonSerializer.Serialize(this);

  public string ToMarkdown()
  {
    var sb = new StringBuilder();

    sb.Append("---");
    sb.AppendLine(CultureInfo.InvariantCulture, $"title: {Title}");
    sb.AppendLine(CultureInfo.InvariantCulture, $"description: {OpenGraphDescription}");
    sb.AppendLine(CultureInfo.InvariantCulture, $"slug: {Slug}");
    sb.AppendLine("---");
    sb.AppendLine();

    sb.AppendLine(Content);

    return sb.ToString();
  }
}