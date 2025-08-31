namespace AiKnowledgeExchange.Tests.GetCounterValue;

using System.Text.Json;
using SharedKernel;

[TestFixture]
public sealed class GetCounterValueBasicFunctionalityTests : GetCounterValueTestBase
{
    private const string CounterName = "test-counter";

    [Test]
    public async Task GivenExistingCounter_WhenExecutingCommandPrintsCounterValue()
    {
        const int counterValue = 10;

        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        var storageFileInfo = new CounterValueStorage(TestDataDir).GetStorageFileInfo();
        var values = new Dictionary<string, int>(StringComparer.Ordinal) { [CounterName] = counterValue };

        await File.WriteAllTextAsync(
            storageFileInfo.FullName,
            JsonSerializer.Serialize(values),
            timeouts.TestTimeoutToken
        );

        host.Run(timeouts.TestTimeoutToken, "get", CounterName, "--data-dir", TestDataDir.FullName);

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Zero);
            var output = host.GetStdout();
            Assert.That(output, Does.Contain($"The counter {CounterName} has value {counterValue}"));
        }
    }
}
