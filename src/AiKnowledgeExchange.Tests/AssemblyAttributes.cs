using AiKnowledgeExchange.Tests;

[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[assembly: Parallelizable(ParallelScope.Children)]

#if WIN
// on Windows, we are seeing a lot of errors due to locked files when running
// tests with unlimited parallelism, so we limit it for more reliability
[assembly: LevelOfParallelism(6)]
#endif

[assembly: FileSystemTransportTestLoggingHook]
