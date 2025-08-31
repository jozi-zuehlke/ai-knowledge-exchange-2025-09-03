namespace AiKnowledgeExchange.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using CliFx.Infrastructure;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "does not matter here")]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "false positive")]
internal sealed class TestConsole : IConsole, IDisposable
{
    private readonly CancellationToken cancellationToken;
    private readonly StringBuilder stderrBuilder = new();
    private readonly StringBuilder stdoutBuilder = new();

    static TestConsole()
    {
        // to support icons in output on Windows
        Console.OutputEncoding = Encoding.UTF8;
    }

    public TestConsole(in CancellationToken cancellationToken)
    {
        this.cancellationToken = cancellationToken;

        Input = new ConsoleReader(this, Console.OpenStandardInput());
        Output = new ConsoleWriter(this, new ConsoleWriteStream(stdoutBuilder));
        Error = new ConsoleWriter(this, new ConsoleWriteStream(stderrBuilder));
    }

    public ConsoleReader Input { get; }

    public bool IsInputRedirected => Console.IsInputRedirected;

    public ConsoleWriter Output { get; }

    public bool IsOutputRedirected => Console.IsOutputRedirected;

    public ConsoleWriter Error { get; }

    public bool IsErrorRedirected => Console.IsErrorRedirected;

    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    public ConsoleColor BackgroundColor
    {
        get => Console.BackgroundColor;
        set => Console.BackgroundColor = value;
    }

    public int WindowWidth
    {
        get => Console.WindowWidth;
        set => Console.WindowWidth = value;
    }

    public int WindowHeight
    {
        get => Console.WindowHeight;
        set => Console.WindowHeight = value;
    }

    public int CursorLeft
    {
        get => Console.CursorLeft;
        set => Console.CursorLeft = value;
    }

    public int CursorTop
    {
        get => Console.CursorTop;
        set => Console.CursorTop = value;
    }

    public ConsoleKeyInfo ReadKey(bool intercept = false) => Console.ReadKey(intercept);

    public void ResetColor() => Console.ResetColor();

    public void Clear() => Console.Clear();

    [SuppressMessage(
        "Design",
        "MA0045:Do not use blocking calls in a sync method (need to make calling method async)",
        Justification = "in sync event handler"
    )]
    public CancellationToken RegisterCancellationHandler() => cancellationToken;

    public void Dispose()
    {
        Input.Dispose();
        Output.Dispose();
        Error.Dispose();
    }

    public string GetStdout() => stdoutBuilder.ToString();

    public string GetStderr() => stderrBuilder.ToString();
}

file sealed class ConsoleWriteStream(StringBuilder stringBuilder) : Stream
{
    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => 0;

    public override long Position { get; set; }

    public override void Flush()
    {
        // nothing to do
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
        var output = Console.OutputEncoding.GetString(buffer, offset, count);

        stringBuilder.Append(output);
    }
}
