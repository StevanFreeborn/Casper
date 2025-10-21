namespace Casper.Console.Write;

internal record WriterAgentResponse(
  [property: JsonPropertyName("post")]
  BlogPost Post
);