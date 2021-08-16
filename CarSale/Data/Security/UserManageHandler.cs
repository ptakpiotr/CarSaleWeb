using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Data.Security
{
    public class UserManageHandler : AuthorizationHandler<UserManageRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserManageRequirement requirement)
        {
            if(context.User.HasClaim(c=>c.Type=="Edit Role") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
