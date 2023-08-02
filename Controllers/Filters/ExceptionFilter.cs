using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MMeetupAPI.Controllers.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            this.logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            this.logger.LogCritical($"Caught in ExceptionFilter {context.Exception.Message}", context.Exception);
            var result = new JsonResult("Something went wrong");
            result.StatusCode = 500;

            context.Result = result;
        }
    }
}
