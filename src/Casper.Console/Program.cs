using Casper.Console.Common;
using Casper.Console.Critique;
using Casper.Console.Edit;
using Casper.Console.Research;
using Casper.Console.Write;

using GeminiDotnet;
using GeminiDotnet.Extensions.AI;

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
  })
  .Build();

var chatClient = host.Services.GetRequiredService<IChatClient>();

var researcher = Researcher.From(chatClient);
var writer = Writer.From(chatClient);
var editor = Editor.From(chatClient);
var critic = Critic.From(chatClient);

var workflow = await new WorkflowBuilder(researcher)
  .AddEdge(researcher, writer)
  .AddEdge(writer, editor)
  .AddEdge(editor, writer, static (object? data) => data is Failure)
  .AddEdge(editor, critic)
  .AddEdge(critic, writer, static (object? data) => data is Failure)
  .WithOutputFrom(critic)
  .BuildAsync<ChatMessage>();

await using var run = await InProcessExecution.StreamAsync(workflow, new ChatMessage(ChatRole.User, "Hello there"));

await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

await foreach (var evt in run.WatchStreamAsync())
{
  if (evt is WorkflowOutputEvent outputEvent)
  {
    Console.WriteLine($"{outputEvent}");
  }
}