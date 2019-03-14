using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OdeToFood.Examples
{
    #region Startup1 - very basic dependency injection
    public class Startup1
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // you don't need to pass the IConfiguration as a parameter, it is automatically injected
            services.AddSingleton<IGreeter, Greeter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              IConfiguration configuration,
                              IGreeter greeter)
        {
            // this formats exceptions so that they are more readable and informative
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Note: you don't normally see app.Run in 'real' applications because it is
            //       so basic you can only do simple hings with it

            // Here's one way to get information from appsettings.json and use it in any method.
            // IConfiguration configuration is available to inject into any method
            app.Run(async (context) =>
            {
                string greeting = configuration["Greeting"];
                await context.Response.WriteAsync(greeting);
            });

            // This example shows how we can create a custom interface and class, and make that
            // object available for dependency injection.  In ConfigureServices, we instruct
            // services on how to construct a Greeter when we inject it.
            app.Run(async (context) =>
            {
                string greeting = greeter.GetMessageOfTheDay();
                await context.Response.WriteAsync(greeting);
            });
        }
    }
    #endregion

    #region Startup2 - 'Use' type middleware, app.Use example
    public class Startup2
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // you don't need to pass the IConfiguration as a parameter, it is automatically injected
            services.AddSingleton<IGreeter, Greeter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              IGreeter greeter,
                              ILogger<Startup> logger)
        {
            // sends user to a welcome page.  This call will consume every routing request,
            // so it doesn't matter what your route is, you always end up here.
            app.UseWelcomePage();

            // Now Welcomepage only gets called if /wp is in the route
            app.UseWelcomePage( new WelcomePageOptions
            {
                Path="/wp"
            });

            // app.Use takes function that takes a request delegate and returns a RequestDelegate

            // A RequestDelegate is something that takes an HTTP context and returns a task,
            //    so it is typically going to be an async method

            // 'next' represents the next piece of middleware, so if I choose to implement next,
            //    I am choosing to pass on the request to the next middleware.
            // This outer function is only invoked once, when the Framework is ready to
            //    set up the pipeline.  It takes next as its parameter and returns a
            //    RequestDelegate.
            app.Use(next =>
            {
                // this is a RequestDelegate.  It is going to return a task.
                //    This inner function is the Middleware, and is invoked once per HTTP request.
                return async context =>
                {
                    logger.LogInformation("Request incoming");

                    // if the request starts with /mym, process it
                    if (context.Request.Path.StartsWithSegments("/mym"))
                    {
                        await context.Response.WriteAsync("Hit!!");
                        logger.LogInformation("Request handled");
                    }
                    // else go on to the next middleware
                    else
                    {
                        await next(context);
                        logger.LogInformation("Response outgoing");
                    }
                };
            });
            // Note: View => Output window to watch this.
            //       In "show output from" pulldown, select ASP.NET Core Web Server
            //       Right-click => Clear All
            //       Then run it.  If /mym is in the URL, it does Hit!!
        }

    }
    #endregion
}
