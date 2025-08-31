namespace AiKnowledgeExchange.Tests.Application;

[TestFixture]
public abstract class ApplicationTestBase
{
    protected ApplicationTestBase()
    {
        TestDataDir = new DirectoryInfo(
            Path.Combine(ApplicationTestSetup.TestDataDir.FullName, GetType().Name, Guid.NewGuid().ToString())
        );
    }

    protected DirectoryInfo TestDataDir { get; }

    [SetUp]
    public void Setup()
    {
        TestDataDir.Create();
    }
}
