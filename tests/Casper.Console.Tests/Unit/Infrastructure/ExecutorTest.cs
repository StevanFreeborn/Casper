namespace Casper.Console.Tests.Unit.Infrastructure;

public class ExecutorTest
{
  protected Mock<AIAgent> MockAgent { get; } = new();
  protected Mock<IWorkflowContext> MockContext { get; } = new();
}