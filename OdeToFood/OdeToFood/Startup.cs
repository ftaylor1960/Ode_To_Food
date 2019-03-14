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
