namespace AiKnowledgeExchange.Tests.GetCounterValue;

[SetUpFixture]
public static class GetCounterValueTestSetup
{
    public static readonly DirectoryInfo TestDataDir = new(
        Path.Combine(TestDataPaths.TestDataDir, "get-counter-value")
    );

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
