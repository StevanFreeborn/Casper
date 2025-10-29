using System.IO.Abstractions;

using Casper.Console.Workflow;

namespace Casper.Console.Tests.Integration.Workflow;

public class WorkflowFactoryTests
{
  private readonly Mock<IFileSystem> _mockFileSystem = new();
  private readonly Mock<AIAgent> _mockAgent = new();
  private readonly WorkflowFactory _sut;

  // TODO: Figure this the fuck out
  // need to basically run the workflow
  // and have it simulate receiving
  // specific messages then validate
  // that the right events have occurred
  // and in the expected order

  public WorkflowFactoryTests()
  {
    _sut = new(
      new(_mockAgent.Object),
      new(_mockAgent.Object),
      new(_mockAgent.Object),
      new(_mockAgent.Object),
      new(_mockAgent.Object, _mockFileSystem.Object)
    );
  }

  [Fact]
  public async Task CreateAsync_WhenCalled_ItShouldReturnAWorkflow()
  {
    var result = await _sut.CreateAsync();

    result.Should().NotBeNull();
    result.ReflectPorts().Should().Contain(kv => kv.Key == "user");
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Interviewer));
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Researcher));
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Writer));
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Editor));
    result.ReflectEdges().Should().Contain(kv => kv.Key == nameof(Critic));
  }
}