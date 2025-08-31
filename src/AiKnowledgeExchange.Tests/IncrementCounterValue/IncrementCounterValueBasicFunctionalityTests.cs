namespace AiKnowledgeExchange.Tests.IncrementCounterValue;

using System.Globalization;
using System.Text.Json;
using SharedKernel;

[TestFixture]
public sealed class IncrementCounterValueBasicFunctionalityTests : IncrementCounterValueTestBase
{
    private const string CounterName = "test-counter";

    [Test]
    public async Task GivenNonExistingCounter_WhenExecutingCommand_IncrementsCounterValue()
    {
        const int counterValue = 10;

        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(
            timeouts.TestTimeoutToken,
            "inc",
            CounterName,
            counterValue.ToString(CultureInfo.InvariantCulture),
            "--data-dir",
            TestDataDir.FullName
        );

        _ = await host.GetCompletionTask();

        var storageFileInfo = new CounterValueStorage(TestDataDir).GetStorageFileInfo();

        var storageFileContent = await File.ReadAllTextAsync(storageFileInfo.FullName, timeouts.TestTimeoutToken);

        var values = JsonSerializer.Deserialize<Dictionary<string, int>>(storageFileContent);

        Assert.That(
            values,
            Is.EquivalentTo(new Dictionary<string, int>(StringComparer.Ordinal) { [CounterName] = counterValue })
        );
    }

    [Test]
    public async Task GivenNonExistingCounter_WhenExecutingCommand_PrintsIncrementedValue()
    {
        const int counterValue = 10;

        using var timeouts = TestTimeouts.Create();

        await using var host = TestHost.Create();

        host.Run(
            timeouts.TestTimeoutToken,
            "inc",
            CounterName,
            counterValue.ToString(CultureInfo.InvariantCulture),
            "--data-dir",
            TestDataDir.FullName
        );

        var statusCode = await host.GetCompletionTask();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.Zero);
            var output = host.GetStdout();
            Assert.That(
                output,
                Does.Contain(
                    $"The counter {CounterName} was incremented by {counterValue} and now has value {counterValue}"
                )
            );
        }
    }
}
