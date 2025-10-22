using Casper.Console.Write;

namespace Casper.Console.Tests.Data;

internal static class TestDataFactory
{

  public static readonly TopicBrief TestTopicBrief = new(
    "Why Your TypeScript Skills Make C# Your Next Superpower",
    "An overview of why C# is an approachable and powerful next step for experienced TypeScript developers.",
    "Due to syntactic similarities and shared paradigms, TypeScript developers can quickly become productive in C# and gain access to a mature, high-performance ecosystem for building applications beyond the browser.",
    "Mid-to-senior level TypeScript developers who are interested in backend development or are looking to expand their skillset beyond the Node.js ecosystem.",
    [
      "Highlight the familiar syntax and language features like classes, interfaces, generics, and async/await.",
      "Showcase the power of the .NET ecosystem, particularly ASP.NET Core for APIs and Entity Framework for database access.",
      "Emphasize the world-class developer experience with tools like Visual Studio and JetBrains Rider."
    ],
    "Informative, pragmatic, and encouraging, framed as a peer-to-peer recommendation.",
    "Challenge the reader to scaffold a new '.NET Minimal API' and build a simple endpoint in under 30 minutes."
  );

  public static async Task<ResearchBrief> TestResearchBrief() => await GetTestObject<ResearchBrief>("researchBrief.json");

  public static async Task<BlogPost> TestBlogPost() => await GetTestObject<BlogPost>("blogPost.json");

  private static async Task<T> GetTestObject<T>(string fileName)
  {
    var json = await GetTestFile(fileName);
    return JsonSerializer.Deserialize<T>(json)
      ?? throw new JsonException($"Unable to parse test {nameof(T)} from file");
  }


  private static Task<string> GetTestFile(string fileName)
  {
    var path = Path.Combine(
      AppContext.BaseDirectory,
      "Data",
      "Files",
      fileName
    );

    return File.ReadAllTextAsync(path);
  }
}