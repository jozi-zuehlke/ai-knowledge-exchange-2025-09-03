namespace AiKnowledgeExchange.SharedKernel;

internal static class SharedKernelServiceCollectionExtensions
{
    public static void AddSharedKernel(this IServiceCollection services, DirectoryInfo dataDir)
    {
        services.AddSingleton(_ => new CounterValueStorage(dataDir));
    }
}
