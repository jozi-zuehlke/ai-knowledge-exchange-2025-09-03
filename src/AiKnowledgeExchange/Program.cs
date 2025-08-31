namespace AiKnowledgeExchange;

using System.Text;
using CliFx;
using GetCounterValue;
using IncrementCounterValue;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // to support icons in output on Windows
        Console.OutputEncoding = Encoding.UTF8;

        using var console = new SystemConsole();
        return await Run(args, console);
    }

    public static async Task<int> Run(string[] args, IConsole console)
    {
        var app = new CliApplicationBuilder()
            .SetTitle("AI knowledge exchange 2025-09-03")
            .SetExecutableName("ai-knowledge-exchange")
            .SetDescription("An example app for incrementing counters.")
            .UseConsole(console)
            .AddCommand<GetCounterValueCliCommand>()
            .AddCommand<IncrementCounterValueCliCommand>()
            .Build();

        return await app.RunAsync(args);
    }
}
