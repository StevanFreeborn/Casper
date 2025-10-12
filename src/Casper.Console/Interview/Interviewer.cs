using System.Text.Json;
using System.Text.Json.Serialization;

using Casper.Console.Common;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

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
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions)
    {
      ChatOptions = new()
      {
        ResponseFormat = ChatResponseFormat.ForJsonSchema<InterviewerResponse>()
      }
    };

    _agent = new ChatClientAgent(client, agentOptions);
    _thread = _agent.GetNewThread();
  }

  public static Interviewer From(IChatClient client) => new(client, null);

  public async ValueTask<Result> HandleAsync(
    ChatMessage message,
    IWorkflowContext context,
    CancellationToken cancellationToken
  )
  {
    var agtRes = await _agent.RunAsync(message.Text, _thread, cancellationToken: cancellationToken);
    var intrRes = JsonSerializer.Deserialize<InterviewerResponse>(agtRes.Text);

    if (intrRes is null)
    {
      throw new ApplicationException("Big problem");
    }

    if (intrRes.NeedMoreInfo)
    {
      await context.SendMessageAsync(new Question(intrRes.Question), cancellationToken: cancellationToken);
      return Result.Fail(intrRes.Question);
    }

    return Result.Ok<Topic>(new(intrRes.TopicSummary));
  }
}

internal record InterviewerResponse(
  [property: JsonPropertyName("needMoreInfo")]
  bool NeedMoreInfo,
  [property: JsonPropertyName("question")]
  string Question,
  [property: JsonPropertyName("topicSummary")]
  string TopicSummary
);

internal record Topic(string Summary);

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