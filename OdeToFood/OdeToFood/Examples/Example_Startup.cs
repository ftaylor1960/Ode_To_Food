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

            app.Run(async (context) =>
            {
                // (optional) if there may be some ambiguity about the type of the data that
                //            you are sending as a response.
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Not found");
            });

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

    #region Startup3 - static files
    public class Startup3
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              ILogger<Startup> logger)
        {
            // this formats exceptions so that they are more readable and informative
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Index.html is the default "default" file.  When you use this, if someone requests
            // the root of the website, they will get index.html (assuming it exists in wwwroot)
            app.UseDefaultFiles();

            // this allows you to serve up static (html) files.  Now, let's say you have
            // a bugs.html file in /wwwroot.  If you go to localhost:12344/bugs.html
            // you will be served that page.  If you only use static files (not default)
            // your URL request must exactly match the file name.
            app.UseStaticFiles();

            // Normally, you need to UseDefaultFiles, then UseStaticFiles to get this to work
            // correctly, but you can use UseFileServer, which installs both.
            app.UseFileServer();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Greetings, Earthling!");
            });
        }
    }
    #endregion

    #region Startup4 - MVC
    public class Startup4
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // This is the service that supports MVC routing
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              ILogger<Startup> logger)
        {
            // this formats exceptions so that they are more readable and informative
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // This allows MVC to detect patterns in the request, and route them to specific
            // files (Controllers).  If we want this to work, we must remember to add the service.
            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Greetings, Earthling!");
            });
        }
    }
    #endregion

    #region Startup5 - MVC -convention based routing (routes.MapRoutes)
    public class Startup5
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // This is the service that supports MVC routing
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
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

                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id:guid}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id:minlength(4)}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id:min(18)}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id:alpha}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{ssn:regex(^\\d{{3}}-\\d{{2}}-\\d{{4}}$)}}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id:required}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id:int:min(1)}");
            });

            // This just makes things a bit neater by giving you a discrete method to put your routes in
            app.UseMvc(ConfigureRoutes);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Greetings, Earthling!");
            });
        }
        // This contains "conventional" route patterns
        private void ConfigureRoutes(IRouteBuilder routeBuilder)
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
    #endregion

    #region Startup6 - MVC -convention based routing (routeBuilder)
    public class Startup6
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // This is the service that supports MVC routing
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            var route = new RouteBuilder(app);
            route.MapGet("", context => context.Response.WriteAsync("This is Default Route"));
            route.MapGet("part1", context => context.Response.WriteAsync("This is Sub child Route"));
            route.MapGet("cricket", context => context.Response.WriteAsync("This is Route Details for Cricket"));
            route.MapGet("detail/{rank}", context => context.Response.WriteAsync($"Route Rank is : {context.GetRouteValue("rank ")}"));
            app.UseRouter(route.Build());

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Greetings, Earthling!");
            });
        }
    }
    #endregion

    #region Startup7 - MVC -convention based routing (routeBuilder, complex)
    public class Startup7
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // This is the service that supports MVC routing
            services.AddMvc();
        }

        // These are some URLs, and their results
        //
        //    /package/create/3 - Hello! Route values: [operation, create], [id, 3]
        //    /package/track/-3 - Hello! Route values: [operation, track], [id, -3]
        //    /package/track/-3/ - Hello! Route values: [operation, track], [id, -3]
        //    /package/track - fail
        //    GET /hello/Joe - Hi, Joe!
        //    POST /hello/Joe - matches get only
        //    GET /hello/Joe/Smith - fail
        public void Configure(IApplicationBuilder app)
        {
            var trackPackageRouteHandler = new RouteHandler(context =>
            {
                var routeValues = context.GetRouteData().Values;
                return context.Response.WriteAsync(
                    $"Hello! Route values: {string.Join(", ", routeValues)}");
            });

            var routeBuilder = new RouteBuilder(app, trackPackageRouteHandler);

            routeBuilder.MapRoute(
                "Track Package Route",
                "package/{operation:regex(^track|create|detonate$)}/{id:int}");

            // MapGet matches only HTTPGET requests.
            routeBuilder.MapGet("hello/{name}", context =>
            {
                var name = context.GetRouteValue("name");
                // The route handler when HTTP GET "hello/<anything>" matches
                // To match HTTP GET "hello/<anything>/<anything>, 
                // use routeBuilder.MapGet("hello/{*name}"
                return context.Response.WriteAsync($"Hi, {name}!");
            });

            var routes = routeBuilder.Build();
            app.UseRouter(routes);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Greetings, Earthling!");
            });
        }
    }
    #endregion


}
