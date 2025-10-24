using Casper.Console;
using Casper.Console.Extensions;
using Casper.Console.Workflow;

var host = Host.CreateDefaultBuilder(args)
  .ConfigureAppConfiguration(static c => c.SetBasePath(AppContext.BaseDirectory))
  .ConfigureServices(static (ctx, srvcs) =>
  {
    srvcs.Configure<GeminiClientOptions>(ctx.Configuration.GetSection(nameof(GeminiClientOptions)));
    srvcs.AddGeminiClient();
    srvcs.AddChatClient();
    srvcs.AddAgents();
    srvcs.AddExecutors();
    srvcs.AddSingleton<IWorkflowFactory, WorkflowFactory>();
    srvcs.AddHostedService<App>();
  })
  .Build();

await host.StartAsync();
