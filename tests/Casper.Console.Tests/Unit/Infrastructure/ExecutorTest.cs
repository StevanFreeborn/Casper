using Spectre.Console;

namespace Casper.Console.Tests.Unit.Infrastructure;

public class ExecutorTest
{
  protected Mock<AIAgent> MockAgent { get; } = new();
  protected Mock<IWorkflowContext> MockContext { get; } = new();
  protected Mock<IAnsiConsole> MockConsole { get; } = new();
}