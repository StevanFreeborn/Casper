namespace Casper.Console.Tests.Integration.Infrastructure;

internal sealed class AgentUnderTest(AIAgent agent, AgentUnderTestOptions options) : AIAgent
{
  private const string ResponsesDirectory = "AgentResponses";
  private const string ResponsesFileExtension = ".txt";
  private readonly AgentUnderTestOptions _options = options;
  private readonly AIAgent _agent = agent;

  public override AgentThread DeserializeThread(JsonElement serializedThread, JsonSerializerOptions? jsonSerializerOptions = null)
    => _agent.DeserializeThread(serializedThread, jsonSerializerOptions);

  public override AgentThread GetNewThread() => _agent.GetNewThread();

  public override async Task<AgentRunResponse> RunAsync(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread = null,
    AgentRunOptions? options = null,
    CancellationToken cancellationToken = default
  )
  {
    var response = await _agent.RunAsync(messages, thread, options, cancellationToken);

    if (_options.CaptureResponses)
    {
      await CaptureResponseAsync(messages, thread, response, cancellationToken);
    }

    return response;
  }

  public override IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread = null,
    AgentRunOptions? options = null,
    CancellationToken cancellationToken = default
  ) => _agent.RunStreamingAsync(messages, thread, options, cancellationToken);

  private async Task CaptureResponseAsync(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread,
    AgentRunResponse response,
    CancellationToken ct
  )
  {
    Directory.CreateDirectory(ResponsesDirectory);

    var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{_agent.Id}{ResponsesFileExtension}";
    var filePath = Path.Combine(ResponsesDirectory, fileName);
    var options = _agent.GetService<AgentRunOptions>();

    var content = $"""
    Timestamp: {DateTime.UtcNow:O}
    ================================
    
    Thread:
    {thread?.Serialize()}

    Messages:
    {messages.Aggregate(
      new StringBuilder(),
      static (sb, message) =>
        sb.AppendLine(CultureInfo.InvariantCulture, $"[{message.Role}]: {message.Text}\n---")
    )}

    Response:
    {response}
    """;

    await File.WriteAllTextAsync(filePath, content, ct);
  }
}