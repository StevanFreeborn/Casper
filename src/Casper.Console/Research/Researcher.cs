namespace Casper.Console.Research;

internal sealed class Researcher :
  ReflectingExecutor<Researcher>,
  IMessageHandler<Success<TopicBrief>, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;
  private readonly IAnsiConsole _console;

  public Researcher(
    [FromKeyedServices(nameof(ResearchAgent))]
    AIAgent agent,
    IAnsiConsole console,
    ExecutorOptions? options = null
  ) : base(nameof(Researcher), options)
  {
    _agent = agent;
    _thread = _agent.GetNewThread();
    _console = console;
  }

  public async ValueTask<Result> HandleAsync(
    Success<TopicBrief> message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    return await _console.Status()
      .StartAsync("Researching...", async ctx =>
      {
        List<ChatMessage> messages = [new ChatMessage(ChatRole.User, message.Value.ToString())];

        var agtRes = await _agent.RunAsync(messages, _thread, cancellationToken: cancellationToken);
        ResearchAgentResponse? researcherRes = null;

        try
        {
          researcherRes = JsonSerializer.Deserialize<ResearchAgentResponse>(agtRes.Text);
        }
        catch (Exception e) when (e is JsonException)
        {
          _console.WriteLine("JSON issue");
        }

        if (researcherRes is null)
        {
          return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
        }

        return Result.Ok(new WriterBrief(message.Value, researcherRes.Brief));
      });
  }
}