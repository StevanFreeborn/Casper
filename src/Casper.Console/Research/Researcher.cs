using System.Text.Json;

using Casper.Console.Common;
using Casper.Console.Interview;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Casper.Console.Research;

internal class Researcher :
  ReflectingExecutor<Researcher>,
  IMessageHandler<Success<Topic>, string>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;

  protected Researcher(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Researcher), options)
  {
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions)
    {
      ChatOptions = new()
      {
        ResponseFormat = ChatResponseFormat.ForJsonSchema<Research>(),
        Tools = [new HostedWebSearchTool()],
      }
    };
    _agent = new ChatClientAgent(client, agentOptions);
    _thread = _agent.GetNewThread();
  }

  public static Researcher From(IChatClient client) => new(client, null);

  public async ValueTask<string> HandleAsync(
    Success<Topic> result,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    var response = await _agent.RunAsync(result.Value.Summary, _thread, cancellationToken: cancellationToken);
    var research = JsonSerializer.Deserialize<Research>(response.Text);

    if (research is null)
    {
      throw new ApplicationException("Uh oh, research is null");
    }

    return research.Results;
  }
}

internal record Research(string Results);