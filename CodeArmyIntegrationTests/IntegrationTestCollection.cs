using Xunit;

namespace CodeArmyIntegrationTests
{
    [CollectionDefinition("IntegrationTestsCollection")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
    {
    }
}
