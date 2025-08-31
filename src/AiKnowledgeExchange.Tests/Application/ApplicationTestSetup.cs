namespace AiKnowledgeExchange.Tests.Application;

[SetUpFixture]
public static class ApplicationTestSetup
{
    public static readonly DirectoryInfo TestDataDir = new(Path.Combine(TestDataPaths.TestDataDir, "application"));

    [OneTimeSetUp]
    public static void SetupGlobal()
    {
        if (TestDataDir.Exists)
        {
            TestDataDir.Delete(recursive: true);
        }

        TestDataDir.Create();
    }

    [OneTimeTearDown]
    public static void TearDownGlobal()
    {
        if (!TestDataDir.Exists)
        {
            return;
        }

        TestDataDir.Delete(recursive: true);
    }
}
