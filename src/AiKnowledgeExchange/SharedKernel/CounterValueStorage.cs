namespace AiKnowledgeExchange.SharedKernel;

internal sealed class CounterValueStorage(DirectoryInfo dataDir)
{
    public FileInfo GetStorageFileInfo() => new(Path.Combine(dataDir.FullName, "counters.json"));
}
