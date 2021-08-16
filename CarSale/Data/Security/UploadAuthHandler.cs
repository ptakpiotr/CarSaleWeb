using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Data.Security
{
    public class UploadAuthHandler : AuthorizationHandler<UploadRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UploadRequirement requirement)
        {
            if (context.User.IsInRole("User") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
