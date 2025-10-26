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
    List<ChatMessage> messages = [message];
    
    var agtRes = await _agent.RunAsync(messages, _thread, cancellationToken: cancellationToken);
    InterviewAgentResponse? interviewerRes;

    try
    {
      interviewerRes = JsonSerializer.Deserialize<InterviewAgentResponse>(agtRes.Text);
    }
    catch (Exception e) when (e is JsonException)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    if (interviewerRes is null)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    if (interviewerRes.NeedMoreInfo)
    {
      var question = new Question(interviewerRes.Question);
      await context.SendMessageAsync(question, cancellationToken: cancellationToken);
      return Result.Fail(question);
    }

    return Result.Ok(interviewerRes.Topic);
  }
}