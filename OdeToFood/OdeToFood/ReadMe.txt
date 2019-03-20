
Since there are so many variations on each technique, I like to create small, simple examples of each and
put them into example files when the code examples need to be in a specific place.  For example, code
that needs to be in the Startup.cs page is put in classes named Startup1, Startup2, etc, and you can
find it in the file named Example_Startup.

Since views need to be in the /Views folder, for my example views I created a /Views/Example folder,
and I gave each view its own separate file.


To use dependency injection to access the configuration file from anywhere
--------------------------------------------------------------------------

First, create your configuration file.  Add a new file, from ASP.NET Core, select an "App Settings File".
In this file, add a line:  "Greeting":  "Greetings, Earthling!",  (look in appsettings.json).

Now in any method (see Startup1 as an example), add a parameter to Configure (IConfiguration configuration).
This is a service which is already available to you everywhere.  Just use "configuration["Greeting"]" to get
the value from the configuration file ("Greetings, Earthling!")


To create a custom service which can be injected into any method
----------------------------------------------------------------

Create a class Greeter, which inherits from the interface IGreeter (see Greeter.cs).  Then, in Startup1, in
the method ConfigureServices(IServiceCollection services), you have to add to services a way to
instantiate a greeter whenever it is injected.  There are three possible scopes for your service:
    services.AddSingleton<IGreeter, Greeter>(); - there can only be one Greeter
	services.AddTransient<IGreeter, Greeter>(); - one will be instantiated every time you inject it
	services.AddScoped<IGreeter, Greeter>(); - one will be created for each HTTP request

Now, in order to use the greeter, just pass IGreeter greeter in as a parameter to any method (see example
in Startup1.Configure in Examples_Startup), and you have an instantiated Greeter object.

Note that in Greeter, there is an instantiator which takes (IConfiguration configuration) as a parameter.
This is the instantiator that will be used in the above examples.  You don't have to specify that
parameter in the service registry because DotNetCore can inject it by default.

Note that services should always be accessed through interfaces.

Another example is /Services/IRestaurantData and /Startup/InMemoryRestaurantData.  Startup8 and
HomeController6 and Example3.cshtml contain examples using it.

Middleware
----------

Middleware lives in Startup.cs, in the Configure method.  The simplest for of middleware (and one not often used) is

   app.Run(async (context) =>
   {
       await context.Response.WriteAsync("This is returned as a response");
   });

By convention, most middleware begins with the word "Use".  For example:
	app.UseWelcomePage(); - this would send users to the welcome page, not matter the URL that they are on
The following sends users to the welcome page only when wp is in the URL
	app.UseWelcomePage(new WelcomePageOption { Path="wp"});

Environment
-----------

In Properties -> launchSettings.json we see two command names defined, IIS Express and OdeToFood.
We have changed OdeToFood to run with its environmental variable as "Production" and IIS Express
set to run as "Development".  What that means is that when our command pulldown (see it in the toolbar)
is "IIS Express", our Development environment variable will be TRUE, and when we are running in OdeToFood,
IsProduction() will be TRUE.  In appsettings.json we have settings and connection strings and what-not
which are used by default.  We have created an appsettings.Development.json file, which VS automatically
detects and uses to override values if we are in that environment (development).

Static files
------------

Example in Startup3 shows you 2 middleware options for serving up static files, and another middleware 
that combines both.  Your static content needs to be in wwwroot.

Routing Middleware (Controllers)
------------------

The goal of routing is to take an HTTP request, and extract from it a controller (class) and an
action (method on that class).

Example in Startup4 shows the simplest form of the routing middleware.  Once you add this middleware
and service, you can start building controllers.

The heart of the MVC system are the controllers.  A controller is a file that ends in "Controller".
They are usually kept in a Controllers folder.  The HomeController is MVC's default controller.

"Routing" is what the middleware does to decide which Controller method to invoke.
    "Convention-based routing" - done from startup (example: Startup5, Startup6, Startup7, AboutController)
	"Attribute-based routing" - attributes on the controllers determine how to route requests (AboutController2-3)

Values returned from actions (controller methods)
-------------------------------------------------

Normally, the type of object you return from a controller will be IActionResult.  This type is unusual, in that it doesn't
write to the response right away.  Instead, it tells MVC "this is what you need to do in order to get the response"
and it writes it to the response later.

This is important because it allows more flexibility in what goes to your response.  You can inspect what is being
sent back, log it, test it or even change it.

Examples of IActionResult, and Controller properties, in HomeController2

You can also return an object or collection of objects.  MVC reads the "accept types header" sent by the client
to define what type of serialization it needs.  This is called "content negotiation".  Example in HomeController3.

You can also return a View.  See HomeController4 for an example of this.

Returning a Razor view
----------------------

Views all need to live in the /Views folder.  You create subfolders with names that match your controllers.
Then create a page with a .cshtml extension.

HomeController4 has a simple example of where MVC looks for Views.

HomeController5 shows examples of how you can pass a model in to the cshtml page.

HomeController6 shows a data services being used to pass a collection of data to teh cshtml page.

Data Models
-----------

A View Model (usually stored in /ViewModels) carries all the information needed by a view
A View Model can be used for "output" or "input"
   Simple example of an "output" ViewModel in Example4.cshtml, HomeIndexViewModel1, HomeController7

An example of views which navigate between pages via links which call routes:
    HomeController7, Example5Index.cshtml, Example5Detail.cshtml