namespace Casper.Console.Tests.Integration.Interview;

public class InterviewAgentEvaluations : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public InterviewAgentEvaluations(TestConfiguration config) : base(config)
  {
    _aut = new(new InterviewAgent(GeminiChatClient), config.AutOptions);
  }
}