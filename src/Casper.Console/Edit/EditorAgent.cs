namespace Casper.Console.Edit;

internal sealed partial class EditorAgent : AgentFacade
{
  public EditorAgent(IChatClient client)
  {
    var agentOptions = new ChatClientAgentOptions(Prompt.SystemInstructions)
    {
      ChatOptions = new()
      {
        ResponseFormat = ChatResponseFormat.ForJsonSchema<EditorAgentResponse>(),
      }
    };

    Agent = new ChatClientAgent(client, agentOptions);
  }
}