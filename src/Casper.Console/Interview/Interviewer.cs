using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Casper.Console.Interview;

internal class Interviewer :
  ReflectingExecutor<Interviewer>,
  IMessageHandler<ChatMessage, string>
{
  protected Interviewer(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Interviewer), options)
  {
  }

  public static Interviewer From(IChatClient client) => new(client, null);

  public async ValueTask<string> HandleAsync(
    ChatMessage message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    System.Console.WriteLine($"Interviewer received message: {message.Text}");
    await Task.Delay(1000, cancellationToken);
    return message.Text;
  }
}