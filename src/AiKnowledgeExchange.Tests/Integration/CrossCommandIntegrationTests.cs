namespace AiKnowledgeExchange.Tests.Integration;

[TestFixture]
public sealed class CrossCommandIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task GivenCounterName_WhenIncrementingAndGettingCounterValue_ReturnedValueIsIncrementedCorrectly()
    {
        using var timeouts = TestTimeouts.Create();

        await using var incHost = TestHost.Create();
        incHost.Run(timeouts.TestTimeoutToken, "inc", "test-counter", "10", "--data-dir", TestDataDir.FullName);
        _ = await incHost.GetCompletionTask();

        await using var getHost = TestHost.Create();
        getHost.Run(timeouts.TestTimeoutToken, "get", "test-counter", "--data-dir", TestDataDir.FullName);
        _ = await getHost.GetCompletionTask();

        var stdout = getHost.GetStdout();
        Assert.That(stdout, Does.Contain("The counter test-counter has value 10"));
    }
}
