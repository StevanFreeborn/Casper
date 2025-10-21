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
    var interviewerRes = JsonSerializer.Deserialize<InterviewAgentResponse>(agtRes.Text);

    if (interviewerRes is null)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    if (interviewerRes.NeedMoreInfo)
    {
      await context.SendMessageAsync(new Question(interviewerRes.Question), cancellationToken: cancellationToken);
      return Result.Fail(interviewerRes.Question);
    }

    return Result.Ok(interviewerRes.Topic);
  }
}