namespace Casper.Console.Edit;

internal sealed class Editor :
  ReflectingExecutor<Editor>,
  IMessageHandler<string, string>
{
  private readonly AIAgent _agent;

  public Editor(
    [FromKeyedServices(nameof(EditorAgent))]
    AIAgent agent,
    ExecutorOptions? options = null
  ) : base(nameof(Editor), options)
  {
    _agent = agent;
  }

  public async ValueTask<string> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
  {
    System.Console.WriteLine($"Editor received message: {message}");
    await Task.Delay(1000, cancellationToken);
    return message;
  }
}