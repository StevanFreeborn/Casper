using System.Runtime.CompilerServices;

namespace Casper.Console.Critique;

internal sealed class Critic :
  ReflectingExecutor<Critic>,
  IMessageHandler<Success<BlogPost>, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;
  private readonly IFileSystem _fileSystem;

  public Critic(
    [FromKeyedServices(nameof(CriticAgent))]
    AIAgent agent,
    IFileSystem fileSystem,
    ExecutorOptions? options = null
  ) : base(nameof(Critic), options)
  {
    _agent = agent;
    _thread = agent.GetNewThread();
    _fileSystem = fileSystem;
  }

  public async ValueTask<Result> HandleAsync(
    Success<BlogPost> message,
    IWorkflowContext context,
    CancellationToken cancellationToken = default
  )
  {
    var blogPostMessage = message.Value.ToMarkdown();
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
      await context.YieldOutputAsync(failureMessage, cancellationToken);
      return Result.Fail(failureMessage);
    }

    if (criticRes.HasFeedback)
    {
      return Result.Fail(new Feedback(message.Value, criticRes.Comments));
    }

    var filePath = _fileSystem.Path.Combine(AppContext.BaseDirectory, $"{message.Value.Title}.md");
    await _fileSystem.File.WriteAllTextAsync(filePath, blogPostMessage, cancellationToken);

    await context.YieldOutputAsync(filePath, cancellationToken);
    return Result.Ok();
  }
}