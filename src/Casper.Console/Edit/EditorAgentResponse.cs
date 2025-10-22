namespace Casper.Console.Edit;

internal sealed record EditorAgentResponse(
  [property: JsonPropertyName("hasFeedback")]
  bool HasFeedback,
  [property: JsonPropertyName("comments")]
  Comment[] Comments
);
