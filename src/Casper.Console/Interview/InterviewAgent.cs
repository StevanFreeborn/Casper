namespace Casper.Console.Interview;

internal sealed partial class InterviewAgent : AgentFacade
{
  public InterviewAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions)
    {
      ChatOptions = new()
      {
        ResponseFormat = ChatResponseFormat.ForJsonSchema<InterviewAgentResponse>()
      }
    };

    Agent = new ChatClientAgent(client, agentOptions);
  }
}