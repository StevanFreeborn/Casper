using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Casper.Console.Edit;

internal class Editor :
  ReflectingExecutor<Editor>,
  IMessageHandler<string, string>
{
  protected Editor(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Editor), options)
  {
  }

  public static Editor From(IChatClient client)
  {
    return new(client, null);
  }

  public async ValueTask<string> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
  {
    System.Console.WriteLine($"Editor received message: {message}");
    await Task.Delay(1000, cancellationToken);
    return message;
  }
}