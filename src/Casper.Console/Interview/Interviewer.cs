namespace Casper.Console.Interview;

internal sealed class Interviewer :
  ReflectingExecutor<Interviewer>,
  IMessageHandler<ChatMessage, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;

  public Interviewer(
    [FromKeyedServices(nameof(InterviewAgent))]
    AIAgent agent,
    ExecutorOptions? options = null
  ) : base(nameof(Interviewer), options)
  {
    _agent = agent;
    _thread = agent.GetNewThread();
  }

  public async ValueTask<Result> HandleAsync(
    ChatMessage message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    var agtRes = await _agent.RunAsync(message.Text, _thread, cancellationToken: cancellationToken);
    var intrRes = JsonSerializer.Deserialize<InterviewAgentResponse>(agtRes.Text);

    if (intrRes is null)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    if (intrRes.NeedMoreInfo)
    {
      await context.SendMessageAsync(new Question(intrRes.Question), cancellationToken: cancellationToken);
      return Result.Fail(intrRes.Question);
    }

    return Result.Ok(intrRes.Topic);
  }
}