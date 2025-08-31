namespace AiKnowledgeExchange.Tests;

using CliFx.Infrastructure;
using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal sealed class TestHost : IAsyncDisposable
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly Action<IServiceCollection>? configureServices;
    private readonly TestConsole console;

    private Task<int>? completionTask;
    private TestLogSink? logSink;

    private TestHost(Action<IServiceCollection>? configureServices)
    {
        this.configureServices = configureServices;
        console = new TestConsole(cancellationTokenSource.Token);
    }

    public IReadOnlyCollection<string> LogEntries => logSink?.LogEntries ?? new List<string>();

    public IConsole Console => console;

    public string GetStdout() => console.GetStdout();

    public string GetStderr() => console.GetStderr();

    public Task<int> GetCompletionTask()
    {
        var t =
            completionTask
            ?? throw new InvalidOperationException("cannot await completion task before running the host");

        completionTask = null;

        return t;
    }

    public async ValueTask DisposeAsync()
    {
        await cancellationTokenSource.CancelAsync();

        if (completionTask is not null)
        {
            var exitCode = await completionTask;
            Assert.That(exitCode, Is.Zero);
        }

        cancellationTokenSource.Dispose();
        console.Dispose();
    }

    public static TestHost Create(Action<IServiceCollection>? configureServices = null) => new(configureServices);

    public void Run(CancellationToken cancellationToken, params string[] args)
    {
        if (completionTask is not null)
        {
            throw new InvalidOperationException("cannot run more than once");
        }

        completionTask = RunInner();

        async Task<int> RunInner()
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationTokenSource.Token,
                cancellationToken
            );

            CancellationTokenRegistration? startupCancellationRegistration = null;

            TestLogPrinter.AddConsole(console);

            var tcs = new TaskCompletionSource<int>();

            try
            {
                MetricsCollector? metricsCollector = null;

                var invocationTask = ProgramInvoker.Invoke(
                    services =>
                    {
                        _ = services.AddLogging(l =>
                            l.ClearProviders().AddTestLogger().SetMinimumLevel(LogLevel.Trace)
                        );

                        configureServices?.Invoke(services);
                    },
                    host =>
                    {
                        logSink = host.Services.GetRequiredService<TestLogSink>();

                        metricsCollector = host.Services.GetRequiredService<MetricsCollector>();

                        console.WriteLine("host initialization complete for test");
                        host.Services.GetRequiredService<ILogger<TestHost>>()
                            .LogInformation("host initialization complete for test");
                    },
                    args.Length is 0 ? args : [.. args, "--no-colors"],
                    console
                );

                var result = await Task.WhenAny(tcs.Task, invocationTask).Unwrap();

                metricsCollector?.PrintSummary(console);

                return result;
            }
            finally
            {
                if (startupCancellationRegistration is not null)
                {
                    await startupCancellationRegistration.Value.DisposeAsync();
                }
            }
        }
    }
}
