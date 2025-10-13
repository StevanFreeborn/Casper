using System.Text.Json;

using Casper.Console.Common;
using Casper.Console.Interview;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Casper.Console.Research;

internal class Researcher :
  ReflectingExecutor<Researcher>,
  IMessageHandler<Success<Topic>, string>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;

  public Researcher(
    [FromKeyedServices(nameof(ResearchAgent))]
    AIAgent agent,
    ExecutorOptions? options = null
  ) : base(nameof(Researcher), options)
  {
    _agent = agent;
    _thread = _agent.GetNewThread();
  }

  public async ValueTask<string> HandleAsync(
    Success<Topic> result,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    var response = await _agent.RunAsync(result.Value.Summary, _thread, cancellationToken: cancellationToken);
    var research = JsonSerializer.Deserialize<ResearchAgentResponse>(response.Text);

    if (research is null)
    {
      throw new ApplicationException("Uh oh, research is null");
    }

    return research.Results;
  }
}
