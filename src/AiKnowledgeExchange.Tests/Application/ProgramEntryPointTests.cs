namespace AiKnowledgeExchange.Tests.Application;

[TestFixture]
public sealed class ProgramEntryPointTests : ApplicationTestBase
{
    [Test]
    public async Task GivenNoArguments_WhenExecutingProgram_ThenShowsHelpMessage()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(timeouts.TestTimeoutToken);

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Zero);
            var output = host.GetStdout();
            Assert.That(output, Does.Contain("AI knowledge exchange 2025-09-03"));
            Assert.That(output, Does.Contain("get"));
            Assert.That(output, Does.Contain("inc"));
        }
    }

    [Test]
    public async Task GivenHelpFlag_WhenExecutingProgram_ThenShowsHelpMessage()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(timeouts.TestTimeoutToken, "--help");

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Zero);
            var output = host.GetStdout();
            Assert.That(output, Does.Contain("USAGE"));
            Assert.That(output, Does.Contain("COMMANDS"));
        }
    }

    [Test]
    public async Task GivenVersionFlag_WhenExecutingProgram_ThenShowsVersionInformation()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(timeouts.TestTimeoutToken, "--version");

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Zero);
            var output = host.GetStdout();
            Assert.That(output, Does.Match(@"\d+\.\d+\.\d+"));
        }
    }

    [Test]
    public async Task GivenInvalidCommand_WhenExecutingProgram_ThenShowsErrorMessage()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(timeouts.TestTimeoutToken, "invalid-command");

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Not.Zero);
            var errorOutput = host.GetStderr();
            Assert.That(errorOutput, Does.Contain("invalid-command").Or.Contain("not found"));
        }
    }

    [Test]
    public async Task GivenCommandHelpFlag_WhenExecutingProgram_ThenShowsCommandSpecificHelp()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(timeouts.TestTimeoutToken, "get", "--help");

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Zero);
            var output = host.GetStdout();
            Assert.That(output, Does.Contain("counterName"));
        }
    }

    [Test]
    public async Task GivenMalformedArguments_WhenExecutingProgram_ThenShowsErrorMessage()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(timeouts.TestTimeoutToken, "get", "test-counter", "--invalid-option");

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Not.Zero);
            var errorOutput = host.GetStderr();
            Assert.That(
                errorOutput,
                Does.Contain("invalid-option").Or.Contain("Unrecognized").Or.Contain("Missing required parameter")
            );
        }
    }
}
