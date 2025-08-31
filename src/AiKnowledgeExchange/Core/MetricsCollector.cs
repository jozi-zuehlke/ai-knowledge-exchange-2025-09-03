namespace AiKnowledgeExchange.Core;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

internal sealed class MetricsCollector
{
    private readonly ConcurrentDictionary<string, MetricData> metrics = [];

    public IDisposable Measure([CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var metricKey = $"{fileName}:{memberName}";

        var sw = Stopwatch.StartNew();
        return new AnonymousDisposable(() =>
        {
            sw.Stop();
            var data = metrics.GetOrAdd(metricKey, _ => new MetricData());
            data.Add(sw.Elapsed);
        });
    }

    public void PrintSummary(IConsole console)
    {
        if (metrics.IsEmpty)
        {
            return;
        }

        console.WriteLine();
        console.WriteLine("=== Metrics Summary ===");
        foreach (var (key, data) in metrics.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
        {
            console.WriteLine(
                $"{key}: Count={data.Count}, Total={data.Total.TotalMilliseconds.ToString("F2", CultureInfo.InvariantCulture)}ms, Avg={data.Average.TotalMilliseconds.ToString("F2", CultureInfo.InvariantCulture)}ms"
            );
        }
    }

    private sealed class AnonymousDisposable(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }

    private sealed class MetricData
    {
        private long count;
        private long totalTicks;

        public long Count => Interlocked.Read(ref count);
        public TimeSpan Total => TimeSpan.FromTicks(Interlocked.Read(ref totalTicks));
        public TimeSpan Average => Count == 0 ? TimeSpan.Zero : TimeSpan.FromTicks(Total.Ticks / Count);

        public void Add(in TimeSpan ts)
        {
            _ = Interlocked.Increment(ref count);
            _ = Interlocked.Add(ref totalTicks, ts.Ticks);
        }
    }
}
