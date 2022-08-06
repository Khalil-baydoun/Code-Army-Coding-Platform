using DataContracts.Users;
using Microsoft.AspNetCore.TestHost;

namespace TestUtilities
{
    public static class TestClientFactory
    {
        public static HttpClient CreateClient(this TestServer testServer, Role role, string? email = null)
        {
            var client = testServer.CreateClient();

            client.DefaultRequestHeaders.Add("Role", role.ToString());

            if (email != null)
            {
                client.DefaultRequestHeaders.Add("Email", email);
            }

            return client;
        }
    }
}
