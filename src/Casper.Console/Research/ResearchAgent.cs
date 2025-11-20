namespace Casper.Console.Research;

internal sealed partial class ResearchAgent : AgentFacade
{
  private static readonly ChatClientAgentRunOptions ResearchOptions = new()
  {
    ChatOptions = new ChatOptions()
    {
      Tools = [new HostedWebSearchTool()],
    }
  };

  private static readonly ChatClientAgentRunOptions FormattingOptions = new()
  {
    ChatOptions = new ChatOptions()
    {
      ResponseFormat = ChatResponseFormat.ForJsonSchema<ResearchAgentResponse>(),
    }
  };

  public ResearchAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions);
    Agent = new ChatClientAgent(client, agentOptions);
  }

  public override async Task<AgentRunResponse> RunAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
  {
    // NOTE: Gemini's API does not support
    // function calling + JSON mode so 
    // have to do this in two steps.
    var rawResponse = await Agent.RunAsync(messages, thread, ResearchOptions, cancellationToken);
    var formattedResponse = await Agent.RunAsync(rawResponse.Text, thread, FormattingOptions, cancellationToken);
    return formattedResponse;
  }
}