namespace AiKnowledgeExchange.Tests.IncrementCounterValue;

[TestFixture]
public sealed class IncrementCounterValueErrorHandlingTests : IncrementCounterValueTestBase
{
    [Test]
    public async Task GivenNonExistentDataDirectory_WhenExecutingCommand_ThenExitsWithErrorMessage()
    {
        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        var nonExistentDir = Path.Combine(TestDataDir.FullName, "nonexistent");

        host.Run(timeouts.TestTimeoutToken, "inc", "test-counter", "10", "--data-dir", nonExistentDir);

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Not.Zero);
            Assert.That(host.GetStderr(), Is.Not.Null.Or.WhiteSpace);
        }
    }
}
