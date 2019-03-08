﻿
To use dependency injection to access the configuration file from anywhere
--------------------------------------------------------------------------

First, create your configuration file.  Add a new file, from ASP.NET Core, select an "App Settings File".
In this file, add a line:  "Greeting":  "Greetings, Earthling!",  (look in appsettings.json).

Now in any method (see Startup.cs as an example), add a parameter to Configure (IConfiguration configuration).
This is a service which is already available to you everywhere.  Just use "configuration["Greeting"]" to get
the value from the configuration file ("Greetings, Earthling!")


To create a custom service which can be injected into any method
----------------------------------------------------------------

Create a class Greeter, which inherits from the interface IGreeter (see Greeter.cs).  Then, in Startup.cs, in
the method ConfigureServices(IServiceCollection services), you have to add to services a way to
instantiate a greeter whenever it is injected.  There are three possible scopes for your service:
    services.AddSingleton<IGreeter, Greeter>(); - there can only be one Greeter
	services.AddTransient<IGreeter, Greeter>(); - one will be instantiated every time you inject it
	services.AddScoped<IGreeter, Greeter>(); - one will be created for each HTTP request

Now, in order to use the greeter, just pass IGreeter greeter in as a parameter to any method (see example
in Startup.Configure), and you have an instantiated Greeter object.

Note that in Greeter, there is an instantiator which takes (IConfiguration configuration) as a parameter.
This is the instantiator that will be used in the above examples.  You don't have to specify that
parameter in the service registry because DotNetCore can inject it by default.

Middleware
----------

Middleware lives in Startup.cs, in the Configure method.  The simplest for of middleware (and one not often used) is

   app.Run(async (context) =>
   {
       await context.Response.WriteAsync("This is returned as a response");
   });

By convention, most middleware begins with the word "Use".  For example:
	app.UseWelcomPage(); - this would send users to the welcome page, not matter the URL that they are on
The following sends users to the welcome page only when wp is in the URL
	app.UseWelcomPage(new WelcomePageOption { Path="wp"});