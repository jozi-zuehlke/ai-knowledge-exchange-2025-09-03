namespace AiKnowledgeExchange.GetCounterValue;

using System.Text.Json;

internal sealed class GetCounterValueHandler(CounterValueStorage storage)
{
    public async Task<int?> GetCounterValue(string counterName, CancellationToken cancellationToken = default)
    {
        var storageFileInfo = storage.GetStorageFileInfo();

        if (!storageFileInfo.Exists)
        {
            return null;
        }

        var content = await File.ReadAllTextAsync(storageFileInfo.FullName, cancellationToken);

        var values = JsonSerializer.Deserialize<Dictionary<string, int?>>(content);

        return values?.GetValueOrDefault(counterName);
    }
}
