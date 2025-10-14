namespace Casper.Console.Critique;

internal sealed class Critic :
  ReflectingExecutor<Critic>,
  IMessageHandler<string>
{
  public Critic(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Critic), options)
  {
  }

  public static Critic From(IChatClient client) => new(client, null);

  public async ValueTask HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
  {
    await context.YieldOutputAsync($"Received message: {message}", cancellationToken);
  }
}