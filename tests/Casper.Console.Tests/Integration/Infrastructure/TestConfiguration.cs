using Microsoft.Extensions.Configuration;

namespace Casper.Console.Tests.Integration.Infrastructure;

internal class TestConfiguration
{
  private const string ConfigFileName = "appsettings.Test.json";
  protected IConfiguration Config { get; }

  public TestConfiguration()
  {
    Config = new ConfigurationBuilder()
      .AddJsonFile(ConfigFileName, optional: false)
      .Build();
  }
}