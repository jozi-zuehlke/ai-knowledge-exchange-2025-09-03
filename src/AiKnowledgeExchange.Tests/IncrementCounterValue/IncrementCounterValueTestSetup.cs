namespace AiKnowledgeExchange.Tests.IncrementCounterValue;

[SetUpFixture]
public static class IncrementCounterValueTestSetup
{
    public static readonly DirectoryInfo TestDataDir = new(
        Path.Combine(TestDataPaths.TestDataDir, "increment-counter-value")
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
