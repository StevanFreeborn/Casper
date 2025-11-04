using System.IO.Abstractions;

using Casper.Console.Workflow;

namespace Casper.Console.Tests.Integration.Workflow;

public class WorkflowFactoryTests
{
  private readonly Mock<IFileSystem> _mockFileSystem = new();
  private readonly Mock<AIAgent> _mockInterviewerAgent = new();
  private readonly Mock<AIAgent> _mockResearcherAgent = new();
  private readonly Mock<AIAgent> _mockWriterAgent = new();
  private readonly Mock<AIAgent> _mockEditorAgent = new();
  private readonly Mock<AIAgent> _mockCriticAgent = new();
  private readonly WorkflowFactory _sut;

  public WorkflowFactoryTests()
  {
    _sut = new(
      new(_mockInterviewerAgent.Object),
      new(_mockResearcherAgent.Object),
      new(_mockWriterAgent.Object),
      new(_mockEditorAgent.Object),
      new(_mockCriticAgent.Object, _mockFileSystem.Object)
    );
  }

  [Fact]
  public async Task CreateAsync_WhenCalled_ItShouldReturnAWorkflow()
  {
    var result = await _sut.CreateAsync();

    result.Should().NotBeNull();
    result.StartExecutorId.Should().Be("user");
    result.ReflectPorts().Should().Contain(kv => kv.Key == "user");
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Interviewer));
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Researcher));
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Writer));
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Editor));
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Critic));
  }

  [Fact]
  public async Task Workflow_WhenItBeginsWithQuestion_ItShouldRequestInputFromUser()
  {
    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.PendingRequests);
    run.OutgoingEvents.Should().Contain(e => e is RequestInfoEvent);
  }

  [Fact]
  public async Task Workflow_WhenUserAnswersQuestion_ItShouldRouteQuestionToInterviewer()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = true
    };

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);
    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.PendingRequests);
    run.NewEvents.Should().Contain(e => e is RequestInfoEvent);


    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenInterviewerAgentFailsToProvideResponse_ItShouldEndWorkflow()
  {
    MockAgentRunResponse(_mockInterviewerAgent, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);
    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);
    run.NewEvents.Should().NotContain(e => e is RequestInfoEvent);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenInterviewerHasEnoughInfo_ItShouldPassTopicBriefToResearcher()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = false
    };

    var researcherAgentResponse = TestDataFactory.ResearchAgentResponse.Generate();

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);
    MockAgentRunResponse(_mockResearcherAgent, researcherAgentResponse);
    MockAgentRunResponse(_mockWriterAgent, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);
    run.NewEvents.Should().NotContain(e => e is RequestInfoEvent);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockResearcherAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(interviewerAgentResponse.Topic.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenInterviewerDoesNotHaveEnoughInfo_ItShouldAskUserFollowUpQuestion()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = true
    };

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);
    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.PendingRequests);
    run.NewEvents.Should().Contain(e => e is RequestInfoEvent);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenResearcherFails_ItShouldEndWorkflow()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = false
    };

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);
    MockAgentRunResponse(_mockResearcherAgent, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);

    run.NewEvents.Should().NotContain(e => e is RequestInfoEvent);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockResearcherAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(interviewerAgentResponse.Topic.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockWriterAgent,
      msgs => true,
      Times.Never()
    );
  }

  [Fact]
  public async Task Workflow_WhenResearcherSucceeds_ItShouldPassResultsToWriter()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = false
    };

    var researcherAgentResponse = TestDataFactory.ResearchAgentResponse.Generate();

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);
    MockAgentRunResponse(_mockResearcherAgent, researcherAgentResponse);
    MockAgentRunResponse(_mockWriterAgent, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockResearcherAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(interviewerAgentResponse.Topic.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockWriterAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(researcherAgentResponse.Brief.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenTheWriterSucceeds_ItShouldPassBlogPostToEditor()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = false
    };

    var researcherAgentResponse = TestDataFactory.ResearchAgentResponse.Generate();
    var writerAgentResponse = TestDataFactory.WriterAgentResponse.Generate();

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);
    MockAgentRunResponse(_mockResearcherAgent, researcherAgentResponse);
    MockAgentRunResponse(_mockWriterAgent, writerAgentResponse);
    MockAgentRunResponse(_mockEditorAgent, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockResearcherAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(interviewerAgentResponse.Topic.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockWriterAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(researcherAgentResponse.Brief.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockEditorAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(writerAgentResponse.Post.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenTheWriterFails_ItShouldEndWorkflow()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = false
    };

    var researcherAgentResponse = TestDataFactory.ResearchAgentResponse.Generate();

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);
    MockAgentRunResponse(_mockResearcherAgent, researcherAgentResponse);
    MockAgentRunResponse(_mockWriterAgent, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockResearcherAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(interviewerAgentResponse.Topic.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockWriterAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(researcherAgentResponse.Brief.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockEditorAgent,
      msgs => true,
      Times.Never()
    );
  }

  [Fact]
  public async Task Workflow_WhenTheEditorFails_ItShouldEndWorkflow()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = false
    };

    var researcherAgentResponse = TestDataFactory.ResearchAgentResponse.Generate();
    var writerAgentResponse = TestDataFactory.WriterAgentResponse.Generate();

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);
    MockAgentRunResponse(_mockResearcherAgent, researcherAgentResponse);
    MockAgentRunResponse(_mockWriterAgent, writerAgentResponse);
    MockAgentRunResponse(_mockEditorAgent, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));

    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockResearcherAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(interviewerAgentResponse.Topic.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockWriterAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(researcherAgentResponse.Brief.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockEditorAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(writerAgentResponse.Post.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenTheEditorHasFeedback_ItShouldPassFeedbackBackToTheWriter()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = false
    };

    var researcherAgentResponse = TestDataFactory.ResearchAgentResponse.Generate();
    var writerAgentResponse = TestDataFactory.WriterAgentResponse.Generate();
    var editorAgentResponse = TestDataFactory.EditorAgentResponse.Generate() with
    {
      HasFeedback = true
    };

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);
    MockAgentRunResponse(_mockResearcherAgent, researcherAgentResponse);
    MockAgentRunResponse(_mockWriterAgent, writerAgentResponse);
    MockAgentRunResponses(_mockEditorAgent, editorAgentResponse, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));
    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockResearcherAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(interviewerAgentResponse.Topic.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockWriterAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(researcherAgentResponse.Brief.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockEditorAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(writerAgentResponse.Post.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Exactly(2)
    );

    var feedback = new Feedback(
      writerAgentResponse.Post,
      editorAgentResponse.Comments
    );

    VerifyAgentRunCalled(
      _mockWriterAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(feedback.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenTheEditorDoesNotHaveFeedback_ItShouldPassBlogPostToCritic()
  {
    var interviewerAgentResponse = TestDataFactory.InterviewAgentResponse.Generate() with
    {
      NeedMoreInfo = false
    };
    var researcherAgentResponse = TestDataFactory.ResearchAgentResponse.Generate();
    var writerAgentResponse = TestDataFactory.WriterAgentResponse.Generate();
    var editorAgentResponse = TestDataFactory.EditorAgentResponse.Generate() with
    {
      HasFeedback = false
    };

    MockAgentRunResponse(_mockInterviewerAgent, interviewerAgentResponse);
    MockAgentRunResponse(_mockResearcherAgent, researcherAgentResponse);
    MockAgentRunResponse(_mockWriterAgent, writerAgentResponse);
    MockAgentRunResponse(_mockEditorAgent, editorAgentResponse);
    MockAgentRunResponse(_mockCriticAgent, string.Empty);

    var workflow = await _sut.CreateAsync();

    await using var run = await RunWorkflowAsync(workflow);

    var re = run.NewEvents.First(e => e is RequestInfoEvent re).As<RequestInfoEvent>();
    var res = re.Request.CreateResponse<ChatMessage>(new(ChatRole.User, "TypeScript"));
    await run.ResumeAsync([res], TestContext.Current.CancellationToken);

    var status = await run.GetStatusAsync(TestContext.Current.CancellationToken);

    status.Should().Be(RunStatus.Idle);

    VerifyAgentRunCalled(
      _mockInterviewerAgent,
      msgs => msgs.Any(msg => msg.Text.Contains("TypeScript", StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockResearcherAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(interviewerAgentResponse.Topic.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockWriterAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(researcherAgentResponse.Brief.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockEditorAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(writerAgentResponse.Post.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );

    VerifyAgentRunCalled(
      _mockCriticAgent,
      msgs => msgs.Any(msg => msg.Text.Contains(writerAgentResponse.Post.ToString(), StringComparison.OrdinalIgnoreCase)),
      Times.Once()
    );
  }

  [Fact]
  public async Task Workflow_WhenTheCriticFails_ItShouldEndWorkflow()
  {
    throw new NotImplementedException();
  }

  [Fact]
  public async Task Workflow_WhenTheCriticProvidesFeedback_ItShouldPassFeedbackBackToTheWriter()
  {
    throw new NotImplementedException();
  }

  [Fact]
  public async Task Workflow_WhenTheCriticHasNoFeedback_ItShouldCompleteTheWorkflow()
  {
    throw new NotImplementedException();
  }

  private static void VerifyAgentRunCalled(
    Mock<AIAgent> mockAgent,
    Func<IEnumerable<ChatMessage>, bool> messagePredicate,
    Times times
  )
  {
    mockAgent.Verify(
      m => m.RunAsync(
        It.Is<IEnumerable<ChatMessage>>(msgs => messagePredicate(msgs)),
        It.IsAny<AgentThread>(),
        It.IsAny<AgentRunOptions>(),
        It.IsAny<CancellationToken>()
      ),
      times
    );
  }

  private static AgentRunResponse MockAgentRunResponse<T>(
    Mock<AIAgent> mockAgent,
    T responseObject
  )
  {
    var runResponse = CreateAgentRunResponse(responseObject);

    mockAgent
      .Setup(m => m.RunAsync(
        It.IsAny<IEnumerable<ChatMessage>>(),
        It.IsAny<AgentThread>(),
        It.IsAny<AgentRunOptions>(),
        It.IsAny<CancellationToken>()
      ))
      .ReturnsAsync(runResponse);

    return runResponse;
  }

  private static AgentRunResponse[] MockAgentRunResponses(
    Mock<AIAgent> mockAgent,
    params IEnumerable<object> responseObjects
  )
  {
    var setup = mockAgent
      .SetupSequence(m => m.RunAsync(
        It.IsAny<IEnumerable<ChatMessage>>(),
        It.IsAny<AgentThread>(),
        It.IsAny<AgentRunOptions>(),
        It.IsAny<CancellationToken>()
      ));

    var responses = responseObjects.Select(CreateAgentRunResponse).ToArray();

    foreach (var runResponse in responses)
    {
      setup = setup.ReturnsAsync(runResponse);
    }

    return responses;
  }

  private static AgentRunResponse CreateAgentRunResponse<T>(T responseObject)
  {
    var responseJson = JsonSerializer.Serialize(responseObject);
    var chatMessageResponse = new ChatMessage(ChatRole.Assistant, responseJson);
    return new AgentRunResponse(chatMessageResponse);
  }

  private static ValueTask<Run> RunWorkflowAsync(Workflow<Question> workflow)
  {
    return InProcessExecution.RunAsync(
      workflow,
      new Question("What would you like to write about?"),
      cancellationToken: TestContext.Current.CancellationToken
    );
  }
}