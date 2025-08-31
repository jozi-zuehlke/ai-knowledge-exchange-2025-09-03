namespace AiKnowledgeExchange.Tests.GetCounterValue;

[TestFixture]
public sealed class GetCounterValueErrorHandlingTests : GetCounterValueTestBase
{
    [Test]
    public async Task GivenMissingCounterNameParameter_WhenExecutingCommand_ThenExitsWithErrorMessage()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(timeouts.TestTimeoutToken, "get", "--data-dir", TestDataDir.FullName);

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Not.Zero);
            Assert.That(host.GetStderr(), Is.Not.Null.Or.WhiteSpace);
        }
    }

    [Test]
    public async Task GivenNonExistentDataDirectory_WhenExecutingCommand_ThenExitsWithErrorMessage()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        var nonExistentDir = Path.Combine(TestDataDir.FullName, "nonexistent");

        host.Run(timeouts.TestTimeoutToken, "get", "test-counter", "--data-dir", nonExistentDir);

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Not.Zero);
            Assert.That(host.GetStderr(), Is.Not.Null.Or.WhiteSpace);
        }
    }
}
