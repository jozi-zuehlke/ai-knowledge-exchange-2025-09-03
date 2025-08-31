namespace AiKnowledgeExchange.Tests;

using System.Collections.Concurrent;
using System.Diagnostics;
using CliFx.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal static class ProgramInvoker
{
    // these are from Microsoft.Extensions.Hosting.HostBuilder, where they are unfortunately private
    private const string HostBuildingDiagnosticListenerName = "Microsoft.Extensions.Hosting";
    private const string HostBuildingEventName = "HostBuilding";
    private const string HostBuiltEventName = "HostBuilt";

    public static async Task<int> Invoke(
        Action<IServiceCollection> configureServices,
        Action<IHost> onHostBuilt,
        string[] args,
        TestConsole console
    )
    {
        using var hostingListener = new HostingListener(
            console,
            builder => builder.ConfigureServices(configureServices),
            onHostBuilt
        );

        using var d = DiagnosticListener.AllListeners.Subscribe(hostingListener);

        return await Program.Run(args, console);
    }

    private sealed class HostingListener(TestConsole console, Action<IHostBuilder> configure, Action<IHost> onHostBuilt)
        : IObserver<DiagnosticListener>,
            IObserver<KeyValuePair<string, object?>>,
            IDisposable
    {
        private readonly ConcurrentBag<IDisposable> disposables = [];

        public void OnError(Exception error) => throw error;

        public void OnNext(DiagnosticListener value)
        {
            if (!string.Equals(value.Name, HostBuildingDiagnosticListenerName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            disposables.Add(value.Subscribe(this));
        }

        public void OnCompleted() { }

        public void OnNext(KeyValuePair<string, object?> value)
        {
            if (string.Equals(value.Key, HostBuildingEventName, StringComparison.OrdinalIgnoreCase))
            {
                var builder = (IHostBuilder)value.Value!;

                if (builder.Properties.TryGetValue(nameof(IConsole), out var c) && ReferenceEquals(c, console))
                {
                    configure(builder);
                }
            }

            if (string.Equals(value.Key, HostBuiltEventName, StringComparison.OrdinalIgnoreCase))
            {
                var host = (IHost)value.Value!;

                if (ReferenceEquals(host.Services.GetRequiredService<IConsole>(), console))
                {
                    onHostBuilt(host);
                }
            }
        }

        public void Dispose()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
