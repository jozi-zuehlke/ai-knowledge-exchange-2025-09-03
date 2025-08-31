namespace AiKnowledgeExchange.Tests;

internal static class TestDataPaths
{
    public static readonly string TestDataDir = Environment.GetEnvironmentVariable(
        "AI_KNOWLEDGE_EXCHANGE_TEST_DATA_DIR"
    )
        is { Length: > 0 } dirFromEnv
        ? dirFromEnv
        : Path.Combine(Directory.GetCurrentDirectory(), ".test-data");
}
