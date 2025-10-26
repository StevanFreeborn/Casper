namespace Casper.Console.Edit;

internal sealed class Editor :
  ReflectingExecutor<Editor>,
  IMessageHandler<Success<BlogPost>, Result>
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
    Success<BlogPost> message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    var agtRes = await _agent.RunAsync(message.Value.ToString(), _thread, cancellationToken: cancellationToken);
    EditorAgentResponse? editorRes;

    try
    {
      editorRes = JsonSerializer.Deserialize<EditorAgentResponse>(agtRes.Text);
    }
    catch (Exception e) when (e is JsonException)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    if (editorRes is null)
    {
      return Result.Fail("Apologies, I couldn't process your request at this time. Please try again later.");
    }

    if (editorRes.HasFeedback)
    {
      return Result.Fail(new Feedback(message.Value, editorRes.Comments));
    }

    return Result.Ok(message);
  }
}

static class MyEnum
{
  public const string Hello = "Hello";
}