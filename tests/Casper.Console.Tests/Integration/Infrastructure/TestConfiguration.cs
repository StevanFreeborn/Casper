
namespace Casper.Console.Tests.Integration.Infrastructure;

public class TestConfiguration
{
  private const string ConfigFileName = "appsettings.Test.json";
  public GeminiClientOptions GeminiOptions { get; }
  public AgentUnderTestOptions AutOptions { get; }

  public TestConfiguration()
  {
    var config = new ConfigurationBuilder()
      .AddJsonFile(ConfigFileName, optional: false)
      .Build();

    GeminiOptions = config.GetSection(nameof(GeminiClientOptions)).Get<GeminiClientOptions>() ??
      throw new InvalidOperationException($"Could not load {nameof(GeminiClientOptions)} from {ConfigFileName}");

    AutOptions = config.GetSection(nameof(AgentUnderTestOptions)).Get<AgentUnderTestOptions>()
      ?? new AgentUnderTestOptions();
  }
}