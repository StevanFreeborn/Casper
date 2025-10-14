
namespace Casper.Console.Interview;

internal record InterviewAgentResponse(
  [property: JsonPropertyName("needMoreInfo")]
  bool NeedMoreInfo,
  [property: JsonPropertyName("question")]
  string Question,
  [property: JsonPropertyName("topic")]
  TopicBrief Topic
);