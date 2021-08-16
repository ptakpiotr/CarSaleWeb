using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Utilities.Hangfire
{
    public static class HangfireMiddlewareExts
    {
        public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HangfireMiddleware>();
        }
    }
}
