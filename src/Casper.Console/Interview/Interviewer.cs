using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using Casper.Console.Common;
using Microsoft.Agents.AI;

namespace Casper.Console.Interview;

internal class Interviewer :
  ReflectingExecutor<Interviewer>,
  IMessageHandler<ChatMessage, Result>
{
  private readonly AIAgent _agent;
  private readonly AgentThread _thread;

  protected Interviewer(
    IChatClient client,
    ExecutorOptions? options = null
  ) : base(nameof(Interviewer), options)
  {
    _agent = new ChatClientAgent(client, Prompt.SystemInstructions);
    _thread = _agent.GetNewThread();
  }

  public static Interviewer From(IChatClient client) => new(client, null);

  public async ValueTask<Result> HandleAsync(
    ChatMessage message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    await Task.Delay(1000, cancellationToken);

    var needMoreInformation = true;

    if (needMoreInformation)
    {
      await context.SendMessageAsync(question, cancellationToken: cancellationToken);
      return Result.Fail(question);
    }

    return Result.Ok<TopicSummary>(new());
  }
}

internal record TopicSummary();

internal class Question : Exception
{
  public Question() : base("Can you tell me more about that?")
  {
  }

  public Question(string message) : base(message)
  {
  }

  public Question(string message, Exception innerException) : base(message, innerException)
  {
  }
}