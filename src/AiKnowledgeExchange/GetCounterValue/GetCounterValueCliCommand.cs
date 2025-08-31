namespace AiKnowledgeExchange.GetCounterValue;

using CliFx.Attributes;

[Command("get")]
internal sealed class GetCounterValueCliCommand : BaseCliCommand
{
    [CommandParameter(0, Name = "counterName", Description = "The name of the counter to get the value for")]
    public required string CounterName { get; init; }

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
                services => services.AddGetCounterValueSlice(),
                async (services, cancellationToken) =>
                {
                    var handler = services.GetRequiredService<GetCounterValueHandler>();
                    var result = await handler.GetCounterValue(CounterName, cancellationToken);

                    console.Write("The counter ");

                    using (console.WithForegroundColor(ConsoleColor.Cyan, ColorsAreDisabled))
                    {
                        console.Write(CounterName);
                    }

                    if (result is null)
                    {
                        console.WriteLine(" does not exist");
                        return;
                    }

                    console.Write(" has value ");

                    using (console.WithForegroundColor(ConsoleColor.Yellow, ColorsAreDisabled))
                    {
                        console.Write(result);
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
                await console.Output.WriteLineAsync(
                    $"getting counter value for counter '{CounterName}' failed: {ex.Message}"
                );
            }

            throw;
        }
    }
}
