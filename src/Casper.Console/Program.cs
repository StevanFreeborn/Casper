using GeminiDotnet;
using GeminiDotnet.Extensions.AI;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var host = Host.CreateDefaultBuilder(args)
  .ConfigureAppConfiguration(c => c.SetBasePath(AppContext.BaseDirectory))
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

var response = await chatClient.GetResponseAsync("Write a cute story about cats.");

Console.WriteLine(response.Text);

