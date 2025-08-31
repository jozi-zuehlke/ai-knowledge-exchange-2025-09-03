namespace AiKnowledgeExchange.Core;

internal static class CoreServiceCollectionExtensions
{
    public static void AddCore(this IServiceCollection services, IConsole console)
    {
        services.AddSingleton(console).AddSingleton<MetricsCollector>();
    }
}
