namespace Casper.Console.Workflow;

internal interface IWorkflowFactory
{
  ValueTask<Workflow<Failure<Question>>> CreateAsync();
}

internal sealed class WorkflowFactory : IWorkflowFactory
{
  private readonly Interviewer _interviewer;
  private readonly Researcher _researcher;
  private readonly Writer _writer;
  private readonly Editor _editor;
  private readonly Critic _critic;

  public WorkflowFactory(
    Interviewer interviewer,
    Researcher researcher,
    Writer writer,
    Editor editor,
    Critic critic
  )
  {
    _interviewer = interviewer;
    _researcher = researcher;
    _writer = writer;
    _editor = editor;
    _critic = critic;
  }

  public ValueTask<Workflow<Failure<Question>>> CreateAsync()
  {
    // NOTE: Not sure if this is correct...
    // Should it be resolved through DI?
    // Should caller pass it?
    var user = RequestPort.Create<Failure<Question>, ChatMessage>("user");

    return new WorkflowBuilder(user)
      .AddEdge(user, _interviewer)
      .AddEdge(_interviewer, user, static (object? data) => data is Failure<Question>)
      .AddEdge(_interviewer, _researcher, static (object? data) => data is Success)
      .AddEdge(_researcher, _writer)
      .AddEdge(_writer, _editor)
      .AddEdge(_editor, _writer, static (object? data) => data is Failure<Feedback>)
      .AddEdge(_editor, _critic, static (object? data) => data is Success)
      .AddEdge(_critic, _writer, static (object? data) => data is Failure<Feedback>)
      .WithOutputFrom(_critic)
      .BuildAsync<Failure<Question>>();
  }
}