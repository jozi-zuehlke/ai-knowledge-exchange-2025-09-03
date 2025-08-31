namespace AiKnowledgeExchange.Tests.GetCounterValue;

public abstract class GetCounterValueTestBase
{
    protected GetCounterValueTestBase()
    {
        TestDataDir = new DirectoryInfo(
            Path.Combine(GetCounterValueTestSetup.TestDataDir.FullName, GetType().Name, Guid.NewGuid().ToString())
        );
    }

    protected DirectoryInfo TestDataDir { get; }

    [SetUp]
    public void Setup()
    {
        TestDataDir.Create();
    }
}
