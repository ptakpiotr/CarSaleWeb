using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Utilities.Hangfire
{
    public class HangfireMiddleware
    {
        private readonly RequestDelegate _next;

        public HangfireMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,HangfireJobs jobs)
        {
            jobs.CallHangfireDeleteOld();
            await _next(context);
        }
    }
}
