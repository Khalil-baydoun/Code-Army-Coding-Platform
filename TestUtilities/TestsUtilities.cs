using DataContracts.Courses;
using DataContracts.Problems;
using DataContracts.Tests;
using DataContracts.Users;
using System.Net.Http.Headers;
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

        public static AddUserRequest CreateRandomUserAddRequest(Role role = Role.User, string password = "testPassword1", string? email = null)
        {
            return new AddUserRequest
            {
                Email = email ?? Guid.NewGuid().ToString() + "@gmail.com",
                FirstName = "test",
                LastName = "bay",
                Role = role,
                Password = password,
            };
        }

        public static bool ListsEqual<T>(List<T>? first, List<T>? second)
        {
            if (first == null || second == null || first.Count != second.Count)
            {
                return false;
            }

            return !(first.Except(second).Any() || second.Except(first).Any());
        }

        public static Course CreateRandomCourse()
        {
            return new Course()
            {
                Name = "TestCourse",
                Description = "This is a testing course"
            };
        }

        public static HttpContent StreamToFileStreamContent(Stream stream, string filename)
        {
            HttpContent fileStreamContent = new StreamContent(stream);
            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = filename,
                FileName = filename
            };

            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return fileStreamContent;
        }

        public static MemoryStream FileToMemoryStream(string filename)
        {
            byte[] byteContent = File.ReadAllBytes("Assets/" + filename);
            return new MemoryStream(byteContent);
        }

        public static HttpContent CreateTestFileUploadContent(string inputFileName, string outputFileName, string problemId)
        {
            return new MultipartFormDataContent
            {
                StreamToFileStreamContent(FileToMemoryStream(inputFileName), "Input"),
                StreamToFileStreamContent(FileToMemoryStream(outputFileName), "Output"),
                { new StringContent(problemId), "ProblemId" }
            };
        }

        public static CreateProblemRequest CreateRandomAddProblemRequest(string author, bool isPublic = false)
        {
            return new CreateProblemRequest()
            {
                GeneralDescription = Guid.NewGuid().ToString(),
                InputDescription = Guid.NewGuid().ToString(),
                OutputDescription = Guid.NewGuid().ToString(),
                Difficulty = Difficulty.Medium,
                AuthorEmail = author,
                Title = Guid.NewGuid().ToString(),
                Tags = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                Hints = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                MemoryLimitInKiloBytes = 1000000,
                SampleInput = Guid.NewGuid().ToString(),
                SampleOutput = Guid.NewGuid().ToString(),
                TimeLimitInMilliseconds = 2000,
                IsPublic = isPublic
            };
        }

        public static async Task<string?> ExtractIdFromResponse(HttpResponseMessage response, string IdName)
        {
            var parsedResponse = await TestsUtilities.ExtractResponse<Dictionary<string, object>>(response);
            return parsedResponse?[IdName].ToString();
        }
    }
}
