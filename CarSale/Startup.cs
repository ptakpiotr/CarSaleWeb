using CarSale.Data;
using CarSale.Data.Security;
using CarSale.Models;
using CarSale.Utilities.Hangfire;
using CarSale.Utilities.Mailing;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale
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
            services.AddDbContext<AppDbContext>(opts=> {
                opts.UseNpgsql(Configuration.GetConnectionString("IdentityConn"));
            }).AddIdentity<ApplicationUser, IdentityRole>(opts=> {
                opts.Password.RequiredLength = 9;
                opts.Password.RequireNonAlphanumeric = true;
                opts.SignIn.RequireConfirmedEmail = true;
            }).AddRoles<IdentityRole>().
                AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<CarsDbContext>(opts => 
                                    opts.UseNpgsql(Configuration.GetConnectionString("CarConn")));

            services.AddControllersWithViews();



            services.AddAuthorization(opts=> {
                opts.AddPolicy("AdminPolicy",policyOpts=>policyOpts.RequireAssertion(assertOpts=> {
                    return assertOpts.User.IsInRole("Admin");
                }));

                opts.AddPolicy("ManagePolicy", policyOpts =>
                {
                    policyOpts.AddRequirements(new UserManageRequirement());
                });

                opts.AddPolicy("UploadPolicy", policyOpts =>
                {
                    policyOpts.AddRequirements(new UploadRequirement());
                });

                opts.AddPolicy("EditPolicy", policyOpts =>
                {
                    policyOpts.AddRequirements(new EditCarRequirement());
                });
                
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ICarsRepo, SqlCarRepo>();

            services.AddSingleton<IAuthorizationHandler,UserManageHandler>();
            services.AddSingleton<IAuthorizationHandler, UploadAuthHandler>();
            services.AddScoped<IAuthorizationHandler, EditCarHandler>();

            services.AddScoped<IEmailSender, FluentEmailSender>();

            services.AddHangfire(db => db.UsePostgreSqlStorage(Configuration.GetConnectionString("HangConn")));
            services.AddScoped<HangfireJobs>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseStatusCodePages();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseHangfireServer();

            app.UseHangfireJobs();
            if (env.IsDevelopment())
            {
                app.UseHangfireDashboard("/dashboard");
            }

            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
