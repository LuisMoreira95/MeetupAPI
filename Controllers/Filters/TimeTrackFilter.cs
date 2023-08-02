using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace MMeetupAPI.Controllers.Filters
{
    public class TimeTrackFilter : IActionFilter
    {
        private readonly ILogger<TimeTrackFilter> logger;
        private Stopwatch stopwatch;

        public TimeTrackFilter(ILogger<TimeTrackFilter> logger)
        {
            this.logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            this.stopwatch.Stop();

            var milliseconds = this.stopwatch.ElapsedMilliseconds;
            var action = context.ActionDescriptor.DisplayName;

            this.logger.LogInformation($"Action [{action}], executed in: {milliseconds} milliseconds");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
        }
    }
}
