namespace Casper.Console.Write;

internal sealed class Writer :
  ReflectingExecutor<Writer>,
  IMessageHandler<Success<WriterBrief>, Result>,
  IMessageHandler<Failure<Feedback>, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;
  private readonly IAnsiConsole _console;

  public Writer(
    [FromKeyedServices(nameof(WriterAgent))]
    AIAgent agent,
    IAnsiConsole console,
    ExecutorOptions? options = null
  ) : base(nameof(Writer), options)
  {
    _agent = agent;
    _thread = _agent.GetNewThread();
    _console = console;
  }

  public async ValueTask<Result> HandleAsync(
    Success<WriterBrief> message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    return await _console.Status()
      .StartAsync("Writing...", async ctx =>
      {
        var briefContext = message.Value.ToString();
        var runOptions = new WriterAgentRunOptions()
        {
          Mode = WriterAgentMode.Draft
        };
        var agtRes = await _agent.RunAsync(briefContext, _thread, runOptions, cancellationToken);
        WriterAgentResponse? writerRes = null;

        try
        {
          writerRes = JsonSerializer.Deserialize<WriterAgentResponse>(agtRes.Text);
        }
        catch (Exception e) when (e is JsonException)
        {
        }

        if (writerRes is null)
        {
          return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
        }

        return Result.Ok(writerRes.Post);
      });
  }

  public async ValueTask<Result> HandleAsync(
    Failure<Feedback> message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    return await _console.Status()
      .StartAsync("Revising...", async ctx =>
      {
        var feedbackContext = message.Exception.ToString();
        var runOptions = new WriterAgentRunOptions()
        {
          Mode = WriterAgentMode.Revision
        };
        var agtRes = await _agent.RunAsync(feedbackContext, _thread, runOptions, cancellationToken);
        WriterAgentResponse? writerRes;

        try
        {
          writerRes = JsonSerializer.Deserialize<WriterAgentResponse>(agtRes.Text);
        }
        catch (Exception e) when (e is JsonException)
        {
          return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
        }

        if (writerRes is null)
        {
          return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
        }

        return Result.Ok(writerRes.Post);
      });
  }
}