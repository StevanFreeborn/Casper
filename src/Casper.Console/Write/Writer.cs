namespace Casper.Console.Write;

internal sealed class Writer :
  ReflectingExecutor<Writer>,
  IMessageHandler<Success<WriterBrief>, Result>,
  IMessageHandler<Feedback, Result>
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

  public async ValueTask<Result> HandleAsync(
    Success<WriterBrief> message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    var briefContext = message.Value.ToString();
    var agtRes = await _agent.RunAsync(briefContext, _thread, cancellationToken: cancellationToken);
    var writerRes = JsonSerializer.Deserialize<WriterAgentResponse>(agtRes.Text);

    if (writerRes is null)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    return Result.Ok(writerRes.Post);
  }

  // TODO: Need to handle feedback using
  // correct system instruction...where
  // should we do that...I think in the
  // WriterAgent?
  public ValueTask<Result> HandleAsync(
    Feedback message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    throw new NotImplementedException();
  }
}