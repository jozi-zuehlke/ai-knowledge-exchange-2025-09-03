namespace AiKnowledgeExchange.Tests;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using NUnit.Framework.Interfaces;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

file sealed class TestLogger(string categoryName, TestLogSink logSink, ConsoleFormatter consoleFormatter) : ILogger
{
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        var logEntry = new LogEntry<TState>(logLevel, categoryName, eventId, state, exception, formatter);
        using var textWriter = new StringWriter();
        consoleFormatter.Write(in logEntry, scopeProvider: null, textWriter);
        logSink.LogEntries.Add(textWriter.ToString());
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel is not LogLevel.None;

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => new NoopDisposable();

    private sealed class NoopDisposable : IDisposable
    {
        public void Dispose() { }
    }
}

file sealed class TestLoggerProvider(
    TestLogSink logSink,
    IEnumerable<ConsoleFormatter> consoleFormatters,
    IOptions<ConsoleLoggerOptions> consoleOptions
) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger(
            categoryName,
            logSink,
            consoleFormatters.Single(f =>
                string.Equals(
                    f.Name,
                    consoleOptions.Value.FormatterName ?? ConsoleFormatterNames.Simple,
                    StringComparison.OrdinalIgnoreCase
                )
            )
        );
    }

    public void Dispose() { }
}

internal sealed class TestLogSink
{
    public TestLogSink() => TestLogPrinter.AddLogSink(this);

    public List<string> LogEntries { get; } = [];
}

internal static class TestLoggingBuilderExtensions
{
    public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder)
    {
        _ = builder.AddSimpleConsole(static o =>
        {
            o.TimestampFormat = "[HH:mm:ss.fff]";
            o.ColorBehavior = LoggerColorBehavior.Disabled;
        });

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TestLoggerProvider>());
        builder.Services.TryAddSingleton(new TestLogSink());

        // remove the console logger provider so that we can control the logging
        // ourselves (we want to only log stuff if a test fails to prevent disk
        // churn with superfluous log data, and also to improve test performance)
        _ = builder.Services.Remove(
            builder.Services.Single(static s =>
                s.ServiceType == typeof(ILoggerProvider) && s.ImplementationType == typeof(ConsoleLoggerProvider)
            )
        );

        return builder;
    }
}

internal static class TestLogPrinter
{
    // a test might spawn multiple hosts, and therefore multiple sinks and consoles
    private static readonly ConcurrentDictionary<string, List<TestLogSink>> LogSinksByTestId = [];
    private static readonly ConcurrentDictionary<string, List<TestConsole>> ConsolesByTestId = [];

    public static void AddLogSink(TestLogSink logSink)
    {
        var testId = TestContext.CurrentContext.Test.ID;

        if (!LogSinksByTestId.TryGetValue(testId, out var existingLogSinks))
        {
            existingLogSinks = new List<TestLogSink>();
            LogSinksByTestId[testId] = existingLogSinks;
        }

        existingLogSinks.Add(logSink);
    }

    public static void AddConsole(TestConsole console)
    {
        var testId = TestContext.CurrentContext.Test.ID;

        if (!ConsolesByTestId.TryGetValue(testId, out var existingConsoles))
        {
            existingConsoles = new List<TestConsole>();
            ConsolesByTestId[testId] = existingConsoles;
        }

        existingConsoles.Add(console);
    }

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "false positive, the console is disposed in the host"
    )]
    public static void PrintLogsIfNecessary()
    {
        var testContext = TestContext.CurrentContext;

        if (testContext.Result.Outcome.Status is not TestStatus.Failed)
        {
            return;
        }

        if (ConsolesByTestId.TryRemove(testContext.Test.ID, out var consoles))
        {
            foreach (var console in consoles)
            {
                var stdout = console.GetStdout();
                var stderr = console.GetStderr();

                Console.WriteLine("stdout:");
                Console.WriteLine(stdout);

                Console.WriteLine("stderr:");
                Console.WriteLine(stderr);
            }
        }

        if (!LogSinksByTestId.TryRemove(testContext.Test.ID, out var logSinks))
        {
            return;
        }

        foreach (var logSink in logSinks)
        {
            Console.WriteLine("log output:");
            Console.Write(string.Concat(logSink.LogEntries));
        }
    }
}

[AttributeUsage(AttributeTargets.Assembly)]
internal sealed class FileSystemTransportTestLoggingHookAttribute : Attribute, ITestAction
{
    public ActionTargets Targets => ActionTargets.Test;

    public void BeforeTest(ITest test)
    {
        // nothing to do here
    }

    public void AfterTest(ITest test)
    {
        if (test.IsSuite)
        {
            return;
        }

        TestLogPrinter.PrintLogsIfNecessary();
    }
}
