using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    #region Startup1 - 'Use' type middleware
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
                              IGreeter greeter)
        {
            // this formats exceptions so that they are more readable and informative
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            // sends user to a welcome page.  This call will consume every routing request,
            // so it doesn't matter what your route is, you always end up here.
            app.UseWelcomePage();

            // Now Welcomepage only gets called if /wp is in the route
            app.UseWelcomePage( new WelcomePageOptions
            {
                Path="/wp"
            });

            app.Run(async (context) =>
            {
                string greeting = greeter.GetMessageOfTheDay();
                await context.Response.WriteAsync(greeting);
            });
        }
    }
    #endregion
}
