namespace Casper.Console.Write;

// TODO: I don't know if I like this...
// I'm almost wondering if it wouldn't make
// more sense to have `Message` objects that
// contain all the context needed for an agent
// to run...

internal sealed partial class WriterAgent : AgentFacade
{
  private static readonly ChatClientAgentRunOptions DraftOptions = new()
  {
    ChatOptions = new ChatOptions()
    {
      Instructions = Prompt.DraftSystemInstructions,
    }
  };

  private static readonly ChatClientAgentRunOptions RevisionOptions = new()
  {
    ChatOptions = new ChatOptions()
    {
      Instructions = Prompt.RevisionSystemInstructions,
    }
  };

  public WriterAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions()
    {
      ChatOptions = new()
      {
        ResponseFormat = ChatResponseFormat.ForJsonSchema<WriterAgentResponse>()
      }
    };

    Agent = new ChatClientAgent(client, agentOptions);
  }

  public override async Task<AgentRunResponse> RunAsync(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread = null,
    AgentRunOptions? options = null,
    CancellationToken cancellationToken = default
  )
  {
    var runOptions = options switch
    {
      WriterAgentRunOptions wao when wao.Mode is WriterAgentMode.Draft => DraftOptions,
      WriterAgentRunOptions wao when wao.Mode is WriterAgentMode.Revision => RevisionOptions,
      _ => throw new NotSupportedException("Unsupported WriterAgent mode.")
    };

    return await Agent.RunAsync(messages, thread, runOptions, cancellationToken);
  }
}

internal sealed class WriterAgentRunOptions : AgentRunOptions
{
  public WriterAgentMode Mode { get; init; }
}

internal enum WriterAgentMode
{
  Draft,
  Revision
}
