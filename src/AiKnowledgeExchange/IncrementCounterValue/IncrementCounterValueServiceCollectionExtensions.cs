namespace AiKnowledgeExchange.IncrementCounterValue;

internal static class IncrementCounterValueServiceCollectionExtensions
{
    public static void AddIncrementCounterValueSlice(this IServiceCollection services)
    {
        services.AddTransient<IncrementCounterValueHandler>();
    }
}
