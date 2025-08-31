namespace AiKnowledgeExchange.Core;

using CliFx;
using CliFx.Attributes;

internal abstract class BaseCliCommand : ICommand
{
    [CommandOption(shortName: 'v', Description = "enable verbose output")]
    public bool Verbosity1 { get; init; }

    [CommandOption("vv", Description = "enable extra verbose output")]
    public bool Verbosity2 { get; init; }

    [CommandOption("vvv", Description = "enable super verbose output")]
    public bool Verbosity3 { get; init; }

    [CommandOption("print-metrics", Description = "print performance metrics at the end of the command")]
    public bool PrintMetrics { get; init; }

    [CommandOption("no-colors", Description = "disable coloring of output")]
    public bool ColorsAreDisabled { get; init; }

    [CommandOption("data-dir", Description = "set the directory to use for storing data")]
    public DirectoryInfo DataDir { get; init; } = new(AppContext.BaseDirectory);

    protected LogLevel LogLevel =>
        (Verbosity1, Verbosity2, Verbosity3) switch
        {
            (_, _, true) => LogLevel.Trace,
            (_, true, false) => LogLevel.Debug,
            (true, false, false) => LogLevel.Information,
            _ => LogLevel.Warning,
        };

    public abstract ValueTask ExecuteAsync(IConsole console);
}
