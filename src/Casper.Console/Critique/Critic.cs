namespace Casper.Console.Critique;

internal sealed class Critic :
  ReflectingExecutor<Critic>,
  IMessageHandler<Success<BlogPost>, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;

  public Critic(
    [FromKeyedServices(nameof(CriticAgent))]
    AIAgent agent,
    ExecutorOptions? options = null
  ) : base(nameof(Critic), options)
  {
    _agent = agent;
    _thread = agent.GetNewThread();
  }

  public async ValueTask<Result> HandleAsync(Success<BlogPost> message, IWorkflowContext context, CancellationToken cancellationToken = default)
  {
    var blogPostMessage = message.Value.ToMarkdown();
    var agtRes = await _agent.RunAsync(blogPostMessage, _thread, cancellationToken: cancellationToken);
    var criticRes = JsonSerializer.Deserialize<CriticAgentResponse>(agtRes.Text);

    if (criticRes is null)
    {
      await context.YieldOutputAsync("Uh oh", cancellationToken);
      return Result.Fail("Uh oh");
    }

    var filePath = Path.Combine(AppContext.BaseDirectory, $"{message.Value.Title}.md");
    await File.WriteAllTextAsync(filePath, blogPostMessage, cancellationToken);

    await context.YieldOutputAsync(filePath, cancellationToken);
    return Result.Ok();
  }
}