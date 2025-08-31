namespace AiKnowledgeExchange.Tests.IncrementCounterValue;

public abstract class IncrementCounterValueTestBase
{
    protected IncrementCounterValueTestBase()
    {
        TestDataDir = new DirectoryInfo(
            Path.Combine(IncrementCounterValueTestSetup.TestDataDir.FullName, GetType().Name, Guid.NewGuid().ToString())
        );
    }

    protected DirectoryInfo TestDataDir { get; }

    [SetUp]
    public void Setup()
    {
        TestDataDir.Create();
    }
}
