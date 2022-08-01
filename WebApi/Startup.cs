using WebApi.Store.Sql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using webapi.Services.Interfaces;
using WebApi.Services.Implementations;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;
using WebApi.Services.Implementations.Settings;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Middlewares;
using SqlMigrations;
using webapi.Store.Interfaces;
using WebApi.SignalR;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
                options.JsonSerializerOptions.IgnoreNullValues = true;
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

            services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddSingleton<IOnlineJudgeService, OnlineJudgeService>();
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
            services.AddSingleton<ICommentService, CommentService>();
            services.AddSingleton<ICommentStore, SqlCommentStore>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddSingleton<IStatisticsStore, SqlStatisticsStore>();
            services.AddSingleton<ISolutionService, SolutionService>();
            services.AddSingleton<ISolutionStore, SqlSolutionStore>();
            services.AddSingleton<IWaReportStore, SqlWrongReportStore>();
            services.AddSingleton<IReportStore, SqlReportStore>();
            services.AddSingleton<IReportService, ReportService>();
            services.Decorate<IOnlineJudgeService, PyLintCodeAnalysis>();

            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
                //opt.UseSqlServer(Configuration.GetConnectionString("CloudConnection"));
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
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
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

            services.Configure<SqlDatabaseSettings>(Configuration.GetSection("SqlDatabaseSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            //app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chat"); })

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
