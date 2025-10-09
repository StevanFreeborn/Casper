
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Casper.Console.Research;

internal class Researcher :
  ReflectingExecutor<Researcher>,
  IMessageHandler<ChatMessage, string>
{
  protected Researcher(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Researcher), options)
  {
  }

  public static Researcher From(IChatClient client)
  {
    return new(client, null);
  }

  public async ValueTask<string> HandleAsync(
    ChatMessage message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    System.Console.WriteLine($"Researcher received message: {message.Text}");
    await Task.Delay(1000, cancellationToken);
    return "Hello from Researcher";
  }
}