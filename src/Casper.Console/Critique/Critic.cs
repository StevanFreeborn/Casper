namespace Casper.Console.Critique;

internal sealed class Critic :
  ReflectingExecutor<Critic>,
  IMessageHandler<Success<BlogPost>, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;
  private readonly IFileSystem _fileSystem;
  private readonly IAnsiConsole _console;

  public Critic(
    [FromKeyedServices(nameof(CriticAgent))]
    AIAgent agent,
    IFileSystem fileSystem,
    IAnsiConsole console,
    ExecutorOptions? options = null
  ) : base(nameof(Critic), options)
  {
    _agent = agent;
    _thread = agent.GetNewThread();
    _fileSystem = fileSystem;
    _console = console;
  }

  public async ValueTask<Result> HandleAsync(
    Success<BlogPost> message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    return await _console.Status()
      .StartAsync("Critiquing...", async ctx =>
      {
        var blogPostMessage = message.Value.ToString();
        var agtRes = await _agent.RunAsync(blogPostMessage, _thread, cancellationToken: cancellationToken);
        CriticAgentResponse? criticRes = null;

        try
        {
          criticRes = JsonSerializer.Deserialize<CriticAgentResponse>(agtRes.Text);
        }
        catch (Exception e) when (e is JsonException)
        {
        }

        if (criticRes is null)
        {
          var failureMessage = "Hmmm I wasn't able to finish critiquing the blog post";
          return Result.Fail(failureMessage);
        }

        if (criticRes.HasFeedback)
        {
          return Result.Fail(new Feedback(message.Value, criticRes.Comments));
        }

        var filePath = _fileSystem.Path.Combine(AppContext.BaseDirectory, $"{message.Value.Title}.md");
        await _fileSystem.File.WriteAllTextAsync(filePath, message.Value.ToMarkdown(), cancellationToken);
        _console.WriteLine(message.Value.ToMarkdown());
        return Result.Ok();
      });
  }
}