namespace Casper.Console.Tests.Integration.Write;

public class WriterAgentTests : IntegrationTest
{
  private readonly AgentUnderTest _aut;

  public WriterAgentTests(TestConfiguration config) : base(config)
  {
    _aut = new(new WriterAgent(GeminiChatClient), config.AutOptions);
  }

  [Fact]
  public async Task RunAsync_WhenCalledInDraftMode_ItShouldReturnAResponse()
  {
    var researchBrief = await TestDataFactory.TestResearchBrief.Value;
    var writerBrief = new WriterBrief(
      TestDataFactory.TestTopicBrief,
      researchBrief
    );
    var writerBriefMsg = writerBrief.ToString();
    var runOptions = new WriterAgentRunOptions()
    {
      Mode = WriterAgentMode.Draft
    };

    var response = await _aut.RunAsync(writerBriefMsg, options: runOptions, cancellationToken: TestContext.Current.CancellationToken);

    response.Should().NotBeNull();
  }

  [Fact]
  public async Task RunAsync_WhenCalledInRevisionMode_ItShouldReturnAResponse()
  {
    var originalPost = await TestDataFactory.TestBlogPost.Value;
    var comments = await TestDataFactory.TestComments.Value;
    var feedback = new Feedback(originalPost, comments);
    var feedbackMsg = feedback.ToString();
    var runOptions = new WriterAgentRunOptions()
    {
      Mode = WriterAgentMode.Revision
    };

    var response = await _aut.RunAsync(feedbackMsg, options: runOptions, cancellationToken: TestContext.Current.CancellationToken);

    response.Should().NotBeNull();
  }
}