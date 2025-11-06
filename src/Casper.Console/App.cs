using Casper.Console.Workflow;

namespace Casper.Console;

internal sealed class App : IHostedLifecycleService
{
  private readonly IWorkflowFactory _factory;

  public App(IWorkflowFactory factory)
  {
    _factory = factory;
  }

  public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  public async Task StartedAsync(CancellationToken cancellationToken)
  {
    var workflow = await _factory.CreateAsync();

    await using var run = await InProcessExecution.StreamAsync(
      workflow,
      Result.Fail(new Question("What would you like to write about?")),
      cancellationToken: cancellationToken
    );

    await foreach (var evt in run.WatchStreamAsync(cancellationToken))
    {
      switch (evt)
      {
        case RequestInfoEvent requestInputEvt:
          var response = AskUserQuestion(requestInputEvt.Request, cancellationToken);
          await run.SendResponseAsync(response);
          break;
        case WorkflowOutputEvent outputEvt:
          System.Console.WriteLine($"Workflow completed with result: {outputEvt.Data}");
          return;
        default:
          break;
      }
    }

    static ExternalResponse AskUserQuestion(ExternalRequest request, CancellationToken ct)
    {
      if (request.DataIs<Failure<Question>>(out var failure))
      {
        string? answer = null;

        System.Console.WriteLine($"Casper: {failure.Exception.Message}");

        while (ct.IsCancellationRequested is false && string.IsNullOrWhiteSpace(answer))
        {
          System.Console.Write("> ");
          answer = System.Console.ReadLine();
        }

        return request.CreateResponse<ChatMessage>(new(ChatRole.User, answer));
      }

      throw new InvalidOperationException("Unknown request data type");
    }
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}