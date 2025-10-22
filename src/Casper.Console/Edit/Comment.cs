namespace Casper.Console.Edit;

internal sealed record Comment(
  [property: JsonPropertyName("originalSnippet")]
  string OriginalSnippet,
  [property: JsonPropertyName("issue")]
  string Issue,
  [property: JsonPropertyName("suggestion")]
  string Suggestion
);