using Casper.Console.Common;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Casper.Console.Interview;

internal sealed class InterviewAgent : AgentFacade
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