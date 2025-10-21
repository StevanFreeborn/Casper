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
);