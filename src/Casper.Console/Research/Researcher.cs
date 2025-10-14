namespace Casper.Console.Research;

internal sealed class Researcher :
  ReflectingExecutor<Researcher>,
  IMessageHandler<Success<TopicBrief>, string>
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

  public async ValueTask<string> HandleAsync(
    Success<TopicBrief> result,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    var response = await _agent.RunAsync(result.Value.ToString(), _thread, cancellationToken: cancellationToken);
    var research = JsonSerializer.Deserialize<ResearchAgentResponse>(response.Text);

    if (research is null)
    {
      throw new ApplicationException("Uh oh, research is null");
    }

    return research.Results;
  }
}