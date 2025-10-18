namespace Casper.Console.Write;

internal sealed partial class WriterAgent : AgentFacade
{
  public WriterAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions)
    {
      ChatOptions = new()
      {
        ResponseFormat = ChatResponseFormat.ForJsonSchema<WriterAgentResponse>()
      }
    };

    Agent = new ChatClientAgent(client, agentOptions);
  }
}
