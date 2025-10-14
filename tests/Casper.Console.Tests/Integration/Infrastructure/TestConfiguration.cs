
namespace Casper.Console.Tests.Integration.Infrastructure;

public class TestConfiguration
{
  private const string ConfigFileName = "appsettings.Test.json";
  public GeminiClientOptions Options { get; }

  public TestConfiguration()
  {
    var config = new ConfigurationBuilder()
      .AddJsonFile(ConfigFileName, optional: false)
      .Build();

    Options = config.GetSection(nameof(GeminiClientOptions)).Get<GeminiClientOptions>() ??
      throw new InvalidOperationException($"Could not load {nameof(GeminiClientOptions)} from {ConfigFileName}");
  }
}