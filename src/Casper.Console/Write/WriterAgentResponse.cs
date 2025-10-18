namespace Casper.Console.Write;

internal record WriterAgentResponse(
  [property: JsonPropertyName("content")]
  string Content
);