using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OdeToFood.Data;
using OdeToFood.Services;

namespace OdeToFood
{
    public class Startup
    {
        private IConfiguration _configuration;

        // IConfiguration is not an injectable service, but it is available at startup
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // you don't need to pass the IConfiguration as a parameter, it is automatically injected
            services.AddSingleton<IGreeter, Greeter>();

            // we need to tell services how to set up the EF service.  Do this once per context.
            services.AddDbContext<OdeToFoodDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("OdeToFood")));
            // change this back to AddScoped because EF is not threadsafe so having a Singleton
            // would be unsafe.
            services.AddScoped<IRestaurantData, SqlRestaurantData>();
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


            // This overload will take action of IRouteBuilder.
            app.UseMvc(ConfigureRoutes);

            app.Run(async (context) =>
            {
                string greeting = greeter.GetMessageOfTheDay();
                await context.Response.WriteAsync($"Not found");
                //await context.Response.WriteAsync($"{greeting} : {env.EnvironmentName}");
            });
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            // MapRoute takes a friendly name for the route, and then a template.
            // The template tells MVC how to pick apart the URL in order to determine
            // the correct controller and method.

            routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
        }
        // This is a "conventional" route pattern
        private void ConfigureRoutes1(IRouteBuilder routeBuilder)
        {
            // MapRoute takes a friendly name for the route, and then a template.
            // The template tells MVC how to pick apart the URL in order to determine
            // the correct controller and method.

            // URL: /Home/Index - goes to HomeController.Index()
            routeBuilder.MapRoute("Default", "{controller}/{action}");

            // URL: admin/Home/Index - goes to HomeController.Index()
            routeBuilder.MapRoute("Default", "admin/{controller}/{action}");

            // URL: admin/Home/Index - goes to HomeController.Index() and can
            //        take an id paramter, but id isn't required.
            routeBuilder.MapRoute("Default", "admin/{controller}/{action}/{id?}");

            // URL: / - goes to HomeController.Index() because if we don't specify
            //          a controller or an action, it uses the default.
            //      /home also makes it to the correct route.
            routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}");
        }
    }

}
