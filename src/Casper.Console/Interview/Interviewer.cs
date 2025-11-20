namespace Casper.Console.Interview;

internal sealed class Interviewer :
  ReflectingExecutor<Interviewer>,
  IMessageHandler<ChatMessage, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;
  private readonly IAnsiConsole _console;

  public Interviewer(
    [FromKeyedServices(nameof(InterviewAgent))]
    AIAgent agent,
    IAnsiConsole console,
    ExecutorOptions? options = null
  ) : base(nameof(Interviewer), options)
  {
    _agent = agent;
    _thread = agent.GetNewThread();
    _console = console;
  }

  public async ValueTask<Result> HandleAsync(
    ChatMessage message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    return await _console.Status()
      .StartAsync("Thinking...", async ctx =>
      {
        List<ChatMessage> messages = [message];

        var agtRes = await _agent.RunAsync(messages, _thread, cancellationToken: cancellationToken);
        InterviewAgentResponse? interviewerRes = null;

        try
        {
          interviewerRes = JsonSerializer.Deserialize<InterviewAgentResponse>(agtRes.Text);
        }
        catch (Exception e) when (e is JsonException)
        {
        }

        if (interviewerRes is null)
        {
          return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
        }

        if (interviewerRes.NeedMoreInfo)
        {
          return Result.Fail(new Question(interviewerRes.Question));
        }

        return Result.Ok(interviewerRes.Topic);
      });
  }
}