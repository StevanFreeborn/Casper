namespace Casper.Console.Research;

internal sealed partial class ResearchAgent : AgentFacade
{
  public ResearchAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions)
    {
      ChatOptions = new()
      {
        Tools = [new HostedWebSearchTool()],
      }
    };

    Agent = new ChatClientAgent(client, agentOptions);
  }
}