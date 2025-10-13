using Casper.Console.Common;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Casper.Console.Research;

internal sealed class ResearchAgent : AgentFacade
{
  public ResearchAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions)
    {
      ChatOptions = new()
      {
        ResponseFormat = ChatResponseFormat.ForJsonSchema<ResearchAgentResponse>(),
        Tools = [new HostedWebSearchTool()],
      }
    };

    Agent = new ChatClientAgent(client, agentOptions);
  }
}