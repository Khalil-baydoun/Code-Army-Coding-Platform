using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using webapi;
using SqlMigrations;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using TestUtilities;
using DataContracts.Users;

namespace CodeArmyIntegrationTests
{
    public sealed class IntegrationTestFixture : IDisposable
    {
        public HttpClient AdminTestClient { get; private set; }
        public TestServer Server { get; private set; }
        public IConfiguration TestConfiguration { get; private set; }
        public GlobalMapper? GlobalMapper { get; private set; }
        private static bool databaseUp = false;

        private static object databaseLockObject = new object();

        public IntegrationTestFixture()
        {
            InitConfiguration();
            Server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(collection =>
                {
                    collection.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                         .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
                })
                .UseConfiguration(TestConfiguration));

            using (var scope = Server.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try

                {
                    var dbContext = services.GetRequiredService<DataContext>();
                    dbContext.Database.Migrate();
                    lock (databaseLockObject)
                    {
                        databaseUp = true;
                    }
                }
                catch (Exception)
                {
                }
            }

            AdminTestClient = Server.CreateClient(Role.Admin);

            var newServices = new ServiceCollection();
            newServices.AddSingleton(new GlobalMapper());
            var serviceProvider = newServices.BuildServiceProvider();
            GlobalMapper = serviceProvider?.GetService<GlobalMapper>();
        }

        public void Dispose()
        {
            if (databaseUp)
            {
                using (var sqlConnection = new SqlConnection(TestConfiguration.GetSection("DatabaseSettings")["ConnectionString"]))
                {
                    var serverConnection = new ServerConnection(sqlConnection);
                    var server = new Server(serverConnection);
                    if (server.Databases.Contains(sqlConnection.Database))
                    {
                        server.KillDatabase(sqlConnection.Database);
                    }
                }

                databaseUp = false;
            }
        }

        private void InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            TestConfiguration = config;
        }
    }
}
