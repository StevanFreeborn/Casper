namespace Casper.Console.Write;

internal sealed class Writer :
  ReflectingExecutor<Writer>,
  IMessageHandler<Success<TopicBrief>, string>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;

  public Writer(
    [FromKeyedServices(nameof(WriterAgent))]
    AIAgent agent,
    ExecutorOptions? options = null
  ) : base(nameof(Writer), options)
  {
    _agent = agent;
    _thread = _agent.GetNewThread();
  }

  public ValueTask<string> HandleAsync(Success<TopicBrief> message, IWorkflowContext context, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}