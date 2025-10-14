namespace Casper.Console.Edit;

internal sealed class Editor :
  ReflectingExecutor<Editor>,
  IMessageHandler<string, string>
{
  public Editor(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Editor), options)
  {
  }

  public static Editor From(IChatClient client) => new(client, null);

  public async ValueTask<string> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
  {
    System.Console.WriteLine($"Editor received message: {message}");
    await Task.Delay(1000, cancellationToken);
    return message;
  }
}