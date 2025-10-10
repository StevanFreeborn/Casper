using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Casper.Console.Critique;

internal class Critic :
  ReflectingExecutor<Critic>,
  IMessageHandler<string>
{
  protected Critic(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Critic), options)
  {
  }

  public static Critic From(IChatClient client)
  {
    return new(client, null);
  }

  public async ValueTask HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
  {
    await context.YieldOutputAsync($"Received message: {message}", cancellationToken);
  }
}