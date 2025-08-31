namespace AiKnowledgeExchange.Tests;

using System.Diagnostics;

internal sealed class TestTimeouts : IDisposable
{
    private TestTimeouts() { }

    public required TimeSpan TestTimeout { get; init; }

    public TimeSpan AssertionTimeout => TimeSpan.FromMilliseconds(AssertionTimeoutInMs);

    public required int AssertionTimeoutInMs { get; init; }

    public static TimeSpan PollingInterval => TimeSpan.FromMilliseconds(PollingIntervalInMs);

    public static int PollingIntervalInMs => 10;

    public required TimeSpan ShortDelay { get; init; }

    public CancellationToken TestTimeoutToken => TimeoutCancellationTokenSource.Token;

    public static bool IsRunningInGithubAction { get; } =
        Environment.GetEnvironmentVariable("GITHUB_ACTION") is not null;

    private CancellationTokenSource TimeoutCancellationTokenSource { get; } = new();

    public void Dispose() => TimeoutCancellationTokenSource.Dispose();

    public static TestTimeouts Create(TimeSpan? testTimeout = null)
    {
        var assertionTimeout = Debugger.IsAttached
            ? TimeSpan.FromMinutes(1)
            : TimeSpan.FromMilliseconds(IsRunningInGithubAction ? 20_000 : 5_000);

        var testHost = new TestTimeouts
        {
            TestTimeout = testTimeout ?? TimeSpan.FromSeconds(IsRunningInGithubAction ? 60 : 20),
            AssertionTimeoutInMs = (int)assertionTimeout.TotalMilliseconds,
            ShortDelay = TimeSpan.FromMilliseconds(IsRunningInGithubAction ? 1_000 : 100),
        };

        if (!Debugger.IsAttached)
        {
            testHost.TimeoutCancellationTokenSource.CancelAfter(testHost.TestTimeout);
        }

        return testHost;
    }
}
