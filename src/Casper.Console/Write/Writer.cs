
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Casper.Console.Write;

internal class Writer :
  ReflectingExecutor<Writer>,
  IMessageHandler<string>
{
  protected Writer(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Writer), options)
  {
  }

  public static Writer From(IChatClient client)
  {
    return new(client, null);
  }

  public async ValueTask HandleAsync(
    string message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    await context.YieldOutputAsync(message, cancellationToken);
  }
}