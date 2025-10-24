namespace Casper.Console.Extensions;

internal static class ServicesExtensions
{
  public static IServiceCollection AddAgents(this IServiceCollection srvcs)
  {
    srvcs.AddKeyedSingleton<AIAgent, InterviewAgent>(nameof(InterviewAgent));
    srvcs.AddKeyedSingleton<AIAgent, ResearchAgent>(nameof(ResearchAgent));
    srvcs.AddKeyedSingleton<AIAgent, WriterAgent>(nameof(WriterAgent));
    srvcs.AddKeyedSingleton<AIAgent, EditorAgent>(nameof(EditorAgent));
    srvcs.AddKeyedSingleton<AIAgent, CriticAgent>(nameof(CriticAgent));
    return srvcs;
  }

  public static IServiceCollection AddExecutors(this IServiceCollection srvcs)
  {
    srvcs.AddSingleton<Interviewer>();
    srvcs.AddSingleton<Researcher>();
    srvcs.AddSingleton<Writer>();
    srvcs.AddSingleton<Editor>();
    srvcs.AddSingleton<Critic>();
    return srvcs;
  }

  public static IServiceCollection AddChatClient(this IServiceCollection srvcs)
  {
    srvcs.AddSingleton<IChatClient, GeminiChatClient>(static sp =>
    {
      var geminiClient = sp.GetRequiredService<GeminiClient>();
      return new GeminiChatClient(geminiClient);
    });

    return srvcs;
  }

  public static IServiceCollection AddGeminiClient(this IServiceCollection srvcs)
  {
    srvcs
      .AddHttpClient(
        nameof(GeminiClient),
        c => c.BaseAddress = new("https://generativelanguage.googleapis.com")
      )
      .AddStandardResilienceHandler(o =>
      {
        // TODO: I think this isn't correct
        o.AttemptTimeout = new()
        {
          Timeout = TimeSpan.FromMinutes(5),
        };

        o.CircuitBreaker = new()
        {
          SamplingDuration = TimeSpan.FromMinutes(10), 
        };

        o.TotalRequestTimeout = new()
        {
          Timeout = TimeSpan.FromMinutes(15),
        };
      });

    srvcs.AddSingleton(static sp =>
    {
      var options = sp.GetRequiredService<IOptions<GeminiClientOptions>>();
      var factory = sp.GetRequiredService<IHttpClientFactory>();
      var httpClient = factory.CreateClient(nameof(GeminiClient));
      return new GeminiClient(httpClient, options.Value);
    });

    return srvcs;
  }
}