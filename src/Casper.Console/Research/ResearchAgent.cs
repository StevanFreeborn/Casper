using Casper.Console.Common;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Casper.Console.Research;

internal sealed class ResearchAgent : AgentFacade
{
  public ResearchAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions("Always search the web to answer questions. Be concise and accurate.")
    {
      ChatOptions = new()
      {
        Tools = [new HostedWebSearchTool()],
      }
    };

    Agent = new ChatClientAgent(client, agentOptions);
  }
}