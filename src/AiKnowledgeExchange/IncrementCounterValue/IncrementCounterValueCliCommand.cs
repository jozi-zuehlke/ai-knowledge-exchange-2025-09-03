namespace AiKnowledgeExchange.IncrementCounterValue;

using CliFx.Attributes;
using Core;

[Command("inc")]
internal sealed class IncrementCounterValueCliCommand : BaseCliCommand
{
    [CommandParameter(0, Name = "counterName", Description = "The name of the counter to get the value for")]
    public required string CounterName { get; init; }

    [CommandParameter(1, Name = "incrementBy", Description = "The amount to increment the counter by")]
    public required int IncrementBy { get; init; }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        CounterNameValidation.ValidateCounterName(CounterName);
        DirectoryValidation.ValidateDirectory(DataDir, "data-dir", "data directory");

        try
        {
            await ProgramHost.RunSingle(
                console,
                LogLevel,
                DataDir,
                PrintMetrics,
                services => services.AddIncrementCounterValueSlice(),
                async (services, cancellationToken) =>
                {
                    var handler = services.GetRequiredService<IncrementCounterValueHandler>();
                    var newCounterValue = await handler.IncrementCounterValue(
                        CounterName,
                        IncrementBy,
                        cancellationToken
                    );

                    console.Write("The counter ");

                    using (console.WithForegroundColor(ConsoleColor.Cyan, ColorsAreDisabled))
                    {
                        console.Write(CounterName);
                    }

                    console.Write(" was incremented by ");

                    using (console.WithForegroundColor(ConsoleColor.Yellow, ColorsAreDisabled))
                    {
                        console.Write(IncrementBy);
                    }

                    console.Write(" and now has value ");

                    using (console.WithForegroundColor(ConsoleColor.Yellow, ColorsAreDisabled))
                    {
                        console.Write(newCounterValue);
                    }

                    console.WriteLine();
                },
                console.RegisterCancellationHandler()
            );
        }
        catch (Exception ex)
        {
            using (console.WithForegroundColor(ConsoleColor.Red, ColorsAreDisabled))
            {
                await console.Output.WriteLineAsync($"incrementing counter '{CounterName}' failed: {ex.Message}");
            }

            throw;
        }
    }
}
