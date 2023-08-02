using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MMeetupAPI.Authorization
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private readonly ILogger<MinimumAgeHandler> logger;

        public MinimumAgeHandler(ILogger<MinimumAgeHandler> logger)
        {
            this.logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var userEmail = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);

            this.logger.LogInformation($"Handle minimum age requirement for: {userEmail}. [dateOfBirth: {dateOfBirth}]");

            if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Today)
            {
                this.logger.LogInformation("Access Granted");
                context.Succeed(requirement);
            }
            else
            {
                this.logger.LogInformation("Access Granted");
            }

            return Task.CompletedTask;
        }

    }
}
