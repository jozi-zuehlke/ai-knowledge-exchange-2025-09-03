namespace AiKnowledgeExchange.GetCounterValue;

internal static class GetCounterValueServiceCollectionExtensions
{
    public static void AddGetCounterValueSlice(this IServiceCollection services)
    {
        services.AddTransient<GetCounterValueHandler>();
    }
}
