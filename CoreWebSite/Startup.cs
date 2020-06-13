using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data.Model;
using Data.Model.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreWebSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // добавление сервисов Idenity
            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                        .AddEntityFrameworkStores<ApplicationContext>();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app
            , IWebHostEnvironment env
            , ILoggerFactory loggerFactory
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();    // подключение аутентификации
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                // получаем конечную точку
                Endpoint endpoint = context.GetEndpoint();

                if (endpoint != null)
                {
                    // получаем шаблон маршрута, который ассоциирован с конечной точкой
                    var routePattern = (endpoint as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern?.RawText;

                    Debug.WriteLine($"Endpoint Name: {endpoint.DisplayName}");
                    Debug.WriteLine($"Route Pattern: {routePattern}");

                    // если конечная точка определена, передаем обработку дальше
                    await next();
                }
                else
                {
                    Debug.WriteLine("Endpoint: null");
                    // если конечная точка не определена, завершаем обработку
                    await context.Response.WriteAsync("Endpoint is not defined");
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            var logFile = Configuration.GetSection("LogFile").Value;
            if (!string.IsNullOrEmpty(logFile))
            {
                var log = Path.Combine(Directory.GetCurrentDirectory(), logFile);
                if (File.Exists(log))
                {
                    File.Delete(log);
                }
                loggerFactory.AddFile(log);
                var logger = loggerFactory.CreateLogger("FileLogger");
                logger.LogInformation("Start application. {0}", DateTime.Now);
            }
        }
    }
}
