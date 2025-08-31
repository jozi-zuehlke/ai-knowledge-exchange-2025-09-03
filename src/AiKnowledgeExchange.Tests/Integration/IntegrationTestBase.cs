namespace AiKnowledgeExchange.Tests.Integration;

[TestFixture]
public abstract class IntegrationTestBase
{
    protected IntegrationTestBase()
    {
        TestDataDir = new DirectoryInfo(
            Path.Combine(IntegrationTestSetup.TestDataDir.FullName, GetType().Name, Guid.NewGuid().ToString())
        );
    }

    protected DirectoryInfo TestDataDir { get; }

    [SetUp]
    public void Setup()
    {
        TestDataDir.Create();
    }
}
