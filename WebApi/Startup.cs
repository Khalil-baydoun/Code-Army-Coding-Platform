using WebApi.Store.Sql;
using webapi.Services.Interfaces;
using WebApi.Services.Implementations;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;
using WebApi.Services.Implementations.Settings;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Middlewares;
using SqlMigrations;
using webapi.Store.Interfaces;
using Microsoft.EntityFrameworkCore;
using webapi.Store.Settings;
using webapi.Services.Implementations;
using System.Text.Json.Serialization;
using Webapi.JudgingQueue;
using Webapi.Services.Implementations.Settings;

namespace webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:3000", "https://aubcode.azurewebsites.net")
                        .AllowCredentials();
                });
            });

            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddSingleton<IOnlineJudgeService, JobeJudgingService>();
            services.AddSingleton<IProblemStore, SqlProblemStore>();
            services.AddSingleton<IProblemService, ProblemService>();
            services.AddSingleton<ITestStore, SqlTestStore>();
            services.AddSingleton<ITestService, TestService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IProblemSetService, ProblemSetService>();
            services.AddSingleton<IProblemSetStore, SqlProblemSetStore>();
            services.AddSingleton<ICourseService, CourseService>();
            services.AddSingleton<ICourseStore, SqlCourseStore>();
            services.AddSingleton<IUserStore, SqlUserStore>();
            services.AddSingleton<IHashingService, PBKDF2HashingService>();
            services.AddSingleton<IAuthenticationService, JwtAuthenticationService>();
            services.AddSingleton<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddSingleton<IStatisticsStore, SqlStatisticsStore>();
            services.AddSingleton<ISolutionService, SolutionService>();
            services.AddSingleton<ISolutionStore, SqlSolutionStore>();
            services.AddSingleton<IWaReportStore, SqlWrongReportStore>();
            services.AddSingleton<ISubmissionQueue, ServiceBusSubmissionQueue>();
            services.AddSingleton<IWaReportService, WaReportService>();

            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetSection("DatabaseSettings")["ConnectionString"]);
            });

            var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ValidIssuer = jwtSettings.Issuer,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = jwtSettings.Audience,
            };

            services.AddSingleton(tokenValidationParameters);
            services.AddSingleton(new GlobalMapper());
            services.AddSingleton<JudgingQueue>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("Admins&Instructors", Policies.AdminsAndInstructorsPolicy());
                config.AddPolicy("Admin", Policies.AdminPolicy());
            });

            services.Configure<DatabaseConnectionSettings>(Configuration.GetSection("DatabaseSettings"));
            services.Configure<ServiceBusSettings>(Configuration.GetSection("ServiceBusSettings"));
            services.Configure<JobeServerSettings>(Configuration.GetSection("JobeServerSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // kickstart queue intialization
            app.ApplicationServices.GetService<ISubmissionQueue>();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
