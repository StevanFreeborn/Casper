namespace Casper.Console.Research;

internal sealed class Researcher :
  ReflectingExecutor<Researcher>,
  IMessageHandler<Success<TopicBrief>, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;

  public Researcher(
    [FromKeyedServices(nameof(ResearchAgent))]
    AIAgent agent,
    ExecutorOptions? options = null
  ) : base(nameof(Researcher), options)
  {
    _agent = agent;
    _thread = _agent.GetNewThread();
  }

  public async ValueTask<Result> HandleAsync(
    Success<TopicBrief> result,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    var agtRes = await _agent.RunAsync(result.Value.ToString(), _thread, cancellationToken: cancellationToken);
    var research = JsonSerializer.Deserialize<ResearchAgentResponse>(agtRes.Text);

    if (research is null)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    return Result.Ok(research.Brief);
  }
}