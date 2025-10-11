using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Casper.Console.Write;

internal class Writer :
  ReflectingExecutor<Writer>,
  IMessageHandler<string, string>
{
  protected Writer(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Writer), options)
  {
  }

  public static Writer From(IChatClient client) => new(client, null);

  public async ValueTask<string> HandleAsync(
    string message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    System.Console.WriteLine($"Writer received message: {message}");
    await Task.Delay(1000, cancellationToken);
    return message;
  }
}