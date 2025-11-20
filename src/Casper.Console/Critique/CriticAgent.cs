namespace Casper.Console.Critique;

internal sealed partial class CriticAgent : AgentFacade
{
  public CriticAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions)
    {
      ChatOptions = new()
      {
        ResponseFormat = ChatResponseFormat.ForJsonSchema<CriticAgentResponse>(),
      }
    };

    Agent = new ChatClientAgent(client, agentOptions);
  }
}