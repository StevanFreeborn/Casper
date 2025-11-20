namespace Casper.Console.Edit;

internal sealed class Editor :
  ReflectingExecutor<Editor>,
  IMessageHandler<Success<BlogPost>, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;
  private readonly IAnsiConsole _console;

  public Editor(
    [FromKeyedServices(nameof(EditorAgent))]
    AIAgent agent,
    IAnsiConsole console,
    ExecutorOptions? options = null
  ) : base(nameof(Editor), options)
  {
    _agent = agent;
    _thread = _agent.GetNewThread();
    _console = console;
  }

  public async ValueTask<Result> HandleAsync(
    Success<BlogPost> message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    return await _console.Status()
      .StartAsync("Editing...", async ctx =>
      {
        var agtRes = await _agent.RunAsync(message.Value.ToString(), _thread, cancellationToken: cancellationToken);
        EditorAgentResponse? editorRes = null;

        try
        {
          editorRes = JsonSerializer.Deserialize<EditorAgentResponse>(agtRes.Text);
        }
        catch (Exception e) when (e is JsonException)
        {
        }

        if (editorRes is null)
        {
          return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
        }

        // if (editorRes.HasFeedback)
        // {
        //   return Result.Fail(new Feedback(message.Value, editorRes.Comments));
        // }

        return Result.Ok(message.Value);
      });
  }
}
