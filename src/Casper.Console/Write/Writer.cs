namespace Casper.Console.Write;

internal sealed class Writer :
  ReflectingExecutor<Writer>,
  IMessageHandler<Success<WriterBrief>, string>
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

  public async ValueTask<string> HandleAsync(
    Success<WriterBrief> message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    var briefContext = message.Value.TopicBrief.ToString() + '\n' + message.Value.ResearchBrief.ToString();
    var agtRes = await _agent.RunAsync(briefContext, _thread, cancellationToken: cancellationToken);
    return agtRes.Text;
  }
}