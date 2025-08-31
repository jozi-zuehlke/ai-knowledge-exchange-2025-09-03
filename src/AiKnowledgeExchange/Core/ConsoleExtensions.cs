namespace AiKnowledgeExchange.Core;

internal static class ConsoleExtensions
{
    public static IDisposable WithForegroundColor(this IConsole console, ConsoleColor color, bool isDisabled)
    {
        if (isDisabled)
        {
            return new NoopDisposable();
        }

        return console.WithForegroundColor(color);
    }

    private sealed class NoopDisposable : IDisposable
    {
        public void Dispose()
        {
            // nothing to do
        }
    }
}
