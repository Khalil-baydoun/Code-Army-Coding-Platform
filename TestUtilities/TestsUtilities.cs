using DataContracts.Users;
using System.Text;
using System.Text.Json;

namespace TestUtilities
{
    public static class TestsUtilities
    {
        public static StringContent ToStringContent(object obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }

        public async static Task<T?> ExtractResponse<T>(HttpResponseMessage resp)
        {
            if (resp != null)
            {
                var contentStream = await resp.Content.ReadAsStreamAsync();
                return JsonSerializer.Deserialize<T>(contentStream);
            }

            return default;
        }

        public static AddUserRequest CreateRandomUserAddRequest(Role role = Role.User, string password = "testPassword1")
        {
            AddUserRequest userProfile = new AddUserRequest
            {
                Email = Guid.NewGuid().ToString() + "@gmail.com",
                FirstName = "test",
                LastName = "bay",
                Role = role,
                Password = password,
            };

            return userProfile;
        }
    }
}
