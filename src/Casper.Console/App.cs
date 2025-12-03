using Spectre.Console.Advanced;

namespace Casper.Console;

internal sealed class App : IHostedLifecycleService
{
  private readonly IWorkflowFactory _factory;
  private readonly IAnsiConsole _console;

  public App(IWorkflowFactory factory, IAnsiConsole console)
  {
    _factory = factory;
    _console = console;
  }

  public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  public async Task StartedAsync(CancellationToken cancellationToken)
  {
    var workflow = _factory.Create();

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
          var response = await AskUserQuestion(requestInputEvt.Request, cancellationToken);
          await run.SendResponseAsync(response);
          break;
        case BlogPostSavedEvent savedEvent:
          _console.WriteLine("Casper: Here you go. Give this draft a review.");
          var panel = new Panel($"[blue link={savedEvent.FilePath}]{savedEvent.FilePath.AbsoluteUri.EscapeMarkup()}[/]");
          _console.Write(panel);
          return;
        default:
          break;
      }
    }
  }

  private async Task<ExternalResponse> AskUserQuestion(ExternalRequest request, CancellationToken ct)
  {
    if (request.DataIs<Failure<Question>>(out var failure))
    {
      string? answer = null;

      _console.MarkupLineInterpolated($"Casper: {failure.Exception.Message}");

      while (ct.IsCancellationRequested is false && string.IsNullOrWhiteSpace(answer))
      {
        try
        {
          answer = await _console.AskAsync<string>(" > ", ct);
        }
        catch (Exception ex) when (ex is OperationCanceledException)
        {
          break;
        }
      }

      return request.CreateResponse<ChatMessage>(new(ChatRole.User, answer));
    }

    throw new InvalidOperationException("Unknown request data type");
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}