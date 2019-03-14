using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OdeToFood
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // you don't need to pass the IConfiguration as a parameter, it is automatically injected
            services.AddSingleton<IGreeter, Greeter>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IHostingEnvironment env,
                              IGreeter greeter,
                              ILogger<Startup> logger)
        {
            // This formats exceptions so that they are more readable and informative.
            // If any middleware that is added after this throws an exception, it is handed.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // This does our app.UseMvc with examples of route templates.
            app.UseMvc(routes =>
            {
                // It does pretty much the same thing as app.UseMvcWithDefaultRoute(),
                // since Home and Index are the "default defaults" for MVC.
                //     If no controller is specified, it defaults to Home
                //     If no action is specified, it defaults to Index()
                //     If there is an id parameter it passes it on, but it is not required
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");

                // this does the same thing, with property names
                routes.MapRoute(
                    name: "default_route",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // this does exactly the same thing, but the parts are completely spelled out
                routes.MapRoute(
                    name: "default_Route",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });


                // this constrains id to be an integer value.  This it will match
                // /Products/Details/17 but not /Products/Details/Apples
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id:int}");


            });

            // This allows MVC to detect patterns in the request, and route them to specific
            // files.  If we want this to work, we must remember to add the service.
            //app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                string greeting = greeter.GetMessageOfTheDay();
                // we are adding the name of the environment variable into the greeting
                // This is typically set to the value of ASPNETCORE_ENVIRONMENT, either
                // in launchSettings.json or Properties -> Debug
                await context.Response.WriteAsync($"{greeting} : {env.EnvironmentName}");
            });
        }
    }

}
