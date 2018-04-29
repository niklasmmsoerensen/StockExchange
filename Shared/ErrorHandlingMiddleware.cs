using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e, logger);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            var projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            logger.Error($"Error in {projectName}", exception);

            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(exception.Message);
        }
    }


    public interface ILogger
    {
        void Debug(string message);
        void Debug(string message, Exception exception);

        void Info(string message);
        void Info(string message, Exception exception);

        void Warn(string message);
        void Warn(string message, Exception exception);

        void Error(string message);
        void Error(string message, Exception exception);
    }
}
