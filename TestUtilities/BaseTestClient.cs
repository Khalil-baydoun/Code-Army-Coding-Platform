
namespace TestUtilities
{
    public static class BaseTestClient
    {
        private const string baseUrl = "http://localhost:44381/api/";

        private static string ConstructRequestUrl(string? endpoint, Dictionary<string, string>? queryParams)
        {
            string requestUrl = baseUrl + (endpoint ?? "");

            if (queryParams != null && queryParams.Count > 0)
            {
                requestUrl += "?" + queryParams.Select(kvp => kvp.Key + "=" + kvp.Value + "&");
                requestUrl.Remove(requestUrl.Length - 1);
            }

            return requestUrl;
        }

        public static async Task<HttpResponseMessage> SendRequestAsync(this HttpClient client, HttpMethod method, string? endpoint = null, HttpContent? content = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null)
        {
            var requestMessage = new HttpRequestMessage(method, ConstructRequestUrl(endpoint, queryParams))
            {
                Content = content,
            };

            if (headers != null)
            {
                foreach (var headerKvp in headers)
                {
                    requestMessage.Headers.Add(headerKvp.Key, headerKvp.Value);
                }
            }

            var resp = await client.SendAsync(requestMessage);
            await EnsureSuccessOrThrowAsync(resp);
            return resp;
        }

        private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                string message = $"{responseMessage.ReasonPhrase}, {await responseMessage.Content.ReadAsStringAsync()}";
                throw new CodeArmyException(message, responseMessage.StatusCode);
            }
        }
    }
}
