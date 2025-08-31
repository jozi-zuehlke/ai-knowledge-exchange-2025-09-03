namespace AiKnowledgeExchange.IncrementCounterValue;

using System.Text.Json;

internal sealed class IncrementCounterValueHandler(
    CounterValueStorage storage,
    ILogger<IncrementCounterValueHandler> logger
)
{
    public async Task<int> IncrementCounterValue(
        string counterName,
        int incrementBy,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogTrace(
            "incrementing counter value for counter '{CounterName}' by {IncrementBy}",
            counterName,
            incrementBy
        );

        var storageFileInfo = storage.GetStorageFileInfo();

        Dictionary<string, int> values;

        if (storageFileInfo.Exists)
        {
            var content = await File.ReadAllTextAsync(storageFileInfo.FullName, cancellationToken);
            values =
                JsonSerializer.Deserialize<Dictionary<string, int>>(content)
                ?? new Dictionary<string, int>(StringComparer.Ordinal);
        }
        else
        {
            values = new Dictionary<string, int>(StringComparer.Ordinal);
        }

        values[counterName] = values.GetValueOrDefault(counterName) + incrementBy;

        await File.WriteAllTextAsync(storageFileInfo.FullName, JsonSerializer.Serialize(values), cancellationToken);

        logger.LogTrace(
            "incremented counter value for counter '{CounterName}' by {IncrementBy} to new value {NewValue}",
            counterName,
            incrementBy,
            values[counterName]
        );

        return values[counterName];
    }
}
