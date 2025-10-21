namespace Casper.Console.Edit;

internal sealed class Editor :
  ReflectingExecutor<Editor>,
  IMessageHandler<BlogPost, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;

  public Editor(
    [FromKeyedServices(nameof(EditorAgent))]
    AIAgent agent,
    ExecutorOptions? options = null
  ) : base(nameof(Editor), options)
  {
    _agent = agent;
    _thread = _agent.GetNewThread();
  }

  public async ValueTask<Result> HandleAsync(
    BlogPost message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    var agtRes = await _agent.RunAsync(message.Content, _thread, cancellationToken: cancellationToken);
    var editorRes = JsonSerializer.Deserialize<EditorAgentResponse>(agtRes.Text);

    if (editorRes is null)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    if (editorRes.HasFeedback)
    {
      return Result.Fail(new Feedback(editorRes.Comments));
    }

    return Result.Ok(message);
  }
}