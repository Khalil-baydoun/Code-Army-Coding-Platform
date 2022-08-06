using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Net;
using WebApi.Exceptions;

namespace WebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,
            IWebHostEnvironment hostingEnvironment)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                int statusCode;
                JObject body = new JObject();

                if (e is TimeoutException)
                {
                    statusCode = (int)HttpStatusCode.ServiceUnavailable;
                    _logger.LogError(e, e.Message);
                }

                else if (e is SqlException)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                }

                else if (e is DbUpdateException){
                    statusCode = (int)HttpStatusCode.BadRequest;
                }

                else if (e is StorageErrorException)
                {
                    var storageErrorException = (StorageErrorException)e;
                    statusCode = storageErrorException.StatusCode;
                }

                else if (e is NotFoundException)
                {
                    statusCode = (int)HttpStatusCode.NotFound;
                }

                else if (e is BadRequestException)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                }

                else if (e is ConflictException)
                {
                    statusCode = (int)HttpStatusCode.Conflict;
                }

                else if (e is ForbiddenException)
                {
                    var forbiddenException = (ForbiddenException)e;
                    statusCode = (int)HttpStatusCode.Forbidden;
                    if (forbiddenException.ForbiddenReason.HasValue)
                    {
                        body.Add("ForbiddenReason", forbiddenException.ForbiddenReason.ToString());
                    }
                }
                else
                {
                    statusCode = 500;
                }

                if (statusCode < 500)
                {
                    _logger.LogWarning(e, e.Message);
                }

                else
                {
                    _logger.LogError(e, e.Message);
                }

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                // we do not propagate exceptions in production to avoid leaking sensitive data
                if (!_hostingEnvironment.IsProduction())
                {
                    body.Add("Exception", e.ToString());
                }

                else
                {
                    body.Add("Message", e.Message);
                }

                await context.Response.WriteAsync(JsonConvert.SerializeObject(body));
            }
        }
    }
}
