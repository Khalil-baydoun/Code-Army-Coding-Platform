using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Xunit;

namespace CodeArmyIntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public abstract class CodeArmyTestBase
    {
        public CodeArmyTestBase(IntegrationTestFixture fixture)
        {
        }
    }
}
