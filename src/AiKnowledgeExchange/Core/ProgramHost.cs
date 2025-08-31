namespace AiKnowledgeExchange.Core;

internal static class ProgramHost
{
    public static async Task RunServices(
        IConsole console,
        LogLevel logLevel,
        DirectoryInfo dataDir,
        bool printPerformanceMetrics,
        Action<IServiceCollection> configureServices,
        CancellationToken cancellationToken
    )
    {
        using var host = CreateHost(console, logLevel, dataDir, configureServices);

        try
        {
            await host.RunAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // ignore
        }

        if (printPerformanceMetrics)
        {
            host.Services.GetRequiredService<MetricsCollector>().PrintSummary(console);
        }
    }

    public static async Task RunSingle(
        IConsole console,
        LogLevel logLevel,
        DirectoryInfo dataDir,
        bool printPerformanceMetrics,
        Action<IServiceCollection> configureServices,
        Func<IServiceProvider, CancellationToken, Task> fn,
        CancellationToken cancellationToken
    )
    {
        using var host = CreateHost(console, logLevel, dataDir, configureServices);

        try
        {
            await fn(host.Services, cancellationToken);

            if (printPerformanceMetrics)
            {
                host.Services.GetRequiredService<MetricsCollector>().PrintSummary(console);
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
    }

    private static IHost CreateHost(
        IConsole console,
        LogLevel logLevel,
        DirectoryInfo dataDir,
        Action<IServiceCollection> configureServices
    )
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        // tag the host builder with the console, which allows determining
        // which host builder belongs to which test run
        hostBuilder.Properties.Add(nameof(IConsole), console);

        return hostBuilder
            .ConfigureLogging(l =>
                l.AddSimpleConsole().SetMinimumLevel(logLevel).AddFilter("Microsoft.*", lvl => lvl >= LogLevel.Error)
            )
            .ConfigureServices(services =>
            {
                services.AddCore(console);
                services.AddSharedKernel(dataDir);

                configureServices(services);
            })
            .Build();
    }
}
