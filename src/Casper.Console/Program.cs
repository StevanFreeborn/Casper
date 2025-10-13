using Casper.Console.Common;
using Casper.Console.Critique;
using Casper.Console.Edit;
using Casper.Console.Interview;
using Casper.Console.Research;
using Casper.Console.Write;

using GeminiDotnet;
using GeminiDotnet.Extensions.AI;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var host = Host.CreateDefaultBuilder(args)
  .ConfigureAppConfiguration(static c => c.SetBasePath(AppContext.BaseDirectory))
  .ConfigureServices(static (ctx, srvcs) =>
  {
    srvcs.Configure<GeminiClientOptions>(ctx.Configuration.GetSection(nameof(GeminiClientOptions)));

    srvcs.AddHttpClient();

    srvcs.AddSingleton<IChatClient, GeminiChatClient>(static sp =>
    {
      var options = sp.GetRequiredService<IOptions<GeminiClientOptions>>();
      var factory = sp.GetRequiredService<IHttpClientFactory>();

      var httpClient = factory.CreateClient();
      httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
      httpClient.Timeout = TimeSpan.FromMinutes(2);
      var geminiClient = new GeminiClient(httpClient, options.Value);
      return new GeminiChatClient(geminiClient);
    });

    // TODO: .AddAgents();
    // TODO: Maybe use agent factory?
    srvcs.AddKeyedSingleton<AIAgent, InterviewAgent>(nameof(InterviewAgent));
    srvcs.AddKeyedSingleton<AIAgent, ResearchAgent>(nameof(ResearchAgent));

    // TODO: .AddExecutors();
    srvcs.AddSingleton<Interviewer>();
    srvcs.AddSingleton<Researcher>();
  })
  .Build();

var chatClient = host.Services.GetRequiredService<IChatClient>();

var user = RequestPort.Create<Question, ChatMessage>("user");
var interviewer = host.Services.GetRequiredService<Interviewer>();
var researcher = host.Services.GetRequiredService<Researcher>();
var writer = Writer.From(chatClient);
var editor = Editor.From(chatClient);
var critic = Critic.From(chatClient);

var workflow = await new WorkflowBuilder(user)
  .AddEdge(user, interviewer)
  .AddEdge(interviewer, user, static (object? data) => data is Question)
  .AddEdge(interviewer, researcher, static (object? data) => data is Success)
  .AddEdge(researcher, writer)
  .AddEdge(writer, editor)
  .AddEdge(editor, writer, static (object? data) => data is Failure)
  .AddEdge(editor, critic)
  .AddEdge(critic, writer, static (object? data) => data is Failure)
  .WithOutputFrom(critic)
  .BuildAsync<Question>();

await using var run = await InProcessExecution.StreamAsync(
  workflow,
  new Question("What would you like to write about?")
);

await foreach (var evt in run.WatchStreamAsync())
{
  switch (evt)
  {
    case RequestInfoEvent requestInputEvt:
      var response = AskUserQuestion(requestInputEvt.Request);
      await run.SendResponseAsync(response);
      break;

    case WorkflowOutputEvent outputEvt:
      Console.WriteLine($"Workflow completed with result: {outputEvt.Data}");
      return;
    default:
      break;
  }
}

static ExternalResponse AskUserQuestion(ExternalRequest request)
{
  if (request.DataIs<Question>(out var question))
  {
    string? answer = null;

    Console.WriteLine($"Casper: {question.Message}");

    while (string.IsNullOrWhiteSpace(answer))
    {
      Console.Write("> ");
      answer = Console.ReadLine();
    }

    return request.CreateResponse<ChatMessage>(new(ChatRole.User, answer));
  }

  throw new InvalidOperationException("Unknown request data type");
}