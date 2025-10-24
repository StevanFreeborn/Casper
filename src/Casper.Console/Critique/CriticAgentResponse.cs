namespace Casper.Console.Critique;

internal sealed record CriticAgentResponse(
  [property: JsonPropertyName("hasFeedback")]
  bool HasFeedback,
  [property: JsonPropertyName("comments")]
  Comment[] Comments
);