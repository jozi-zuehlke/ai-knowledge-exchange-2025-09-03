namespace AiKnowledgeExchange.Tests.Integration;

[SetUpFixture]
public static class IntegrationTestSetup
{
    public static readonly DirectoryInfo TestDataDir = new(Path.Combine(TestDataPaths.TestDataDir, "integration"));

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
