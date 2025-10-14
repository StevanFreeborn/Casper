namespace Casper.Console.Common;

internal abstract class AgentFacade : AIAgent
{
  // TODO: This not good...but need move on
  protected AIAgent Agent { get; init; } = default!;

  public override AgentThread DeserializeThread(JsonElement serializedThread, JsonSerializerOptions? jsonSerializerOptions = null)
    => Agent.DeserializeThread(serializedThread, jsonSerializerOptions);

  public override AgentThread GetNewThread() => Agent.GetNewThread();

  public override Task<AgentRunResponse> RunAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
    => Agent.RunAsync(messages, thread, options, cancellationToken);

  public override IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
    => Agent.RunStreamingAsync(messages, thread, options, cancellationToken);
}