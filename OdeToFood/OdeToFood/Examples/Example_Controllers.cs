using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OdeToFood.Models;
using OdeToFood.Services;
using OdeToFood.ViewModels;

namespace OdeToFood.Examples
{
    #region HomeController1
    // The name of this controller (HomeController) is significant.  MVC uses HomeController 
    // as its default, so if the request does not ask for a specific controller, MVC will 
    // try to route to the Home controller.
    public class HomeController1
    {
        // Index is the default method in a controller.  If MVC knows what controller to
        // invoke but doesn't have a a specificmethod, it will try to call the Index method
        public string Index()
        {
            // we can just return a simple string and it will show up on a web page
            return "Hello from the home controller";
        }
    }
    #endregion

    #region AboutController1 (used with conventional routing)
    // This is what we will see, depending on what URL we type, if we are set up to use
    // conventional routing.  See examples of this in Startup5
    //
    //  /about            - we see nothing
    //  /about/phone      - we see "1+555+555-5555"
    //  /about/address    - we see "USA"
    //
    public class AboutController
    {
        public string Phone()
        {
            return "1+555+555-5555";
        }
        public string Address()
        {
            return "USA";
        }
    }
    #endregion

    #region AboutController2 (used with attribute based routing
    // this tells MVC that this controller is called when /about is the first part of the URL
    [Route("about")]
    public class AboutController2
    {
        // This tells MVC that Phone is used as the default action
        // This method is called when the URL is /about
        [Route("")]
        public string Phone()
        {
            return "1+555+555-5555";
        }
        // This is called when the URL is /about/address
        [Route("address")]
        public string Address()
        {
            return "USA";
        }
    }
    #endregion

    #region AboutController3 (attribute with special tokens; controller and action)
    // this tells MVC that this controller is called when /about is the first part of the URL,
    // because the controller name is "about"
    [Route("[controller]")]
    public class AboutController3a
    {
        // This tells MVC that Phone is used as the default action
        // This method is called when the URL is /about
        [Route("")]
        public string Phone()
        {
            return "1+555+555-5555";
        }
        // This is called when the URL is /about/address becasue the name of the method is "address"
        [Route("[action]")]
        public string Address()
        {
            return "USA";
        }
    }
    // OR you can just put both tokens together
    [Route("[controller]/[action]")]
    public class AboutController3b
    {
        public string Phone()
        {
            return "1+555+555-5555";
        }
        public string Address()
        {
            return "USA";
        }
    }
    // NOTE: you can also put literal text in your route
    //    [Route("company/[controller]/[action]")] - would respond to URL /company/about/phone

    #endregion

    #region HomeController2 (properties of Controller class, ways to return IActionResult)
    public class HomeController2 : Controller
    {
        public string Index()
        {
            // When you inherit from the Controller class, there are many more properties available to you
            var body = this.Request.Body;
            var headers = this.Request.Headers;
            var method = this.Request.Method; // GET
            var path = this.Request.Path;   // {/}
            string contentType = this.Request.ContentType;

            var routeData = this.RouteData;
            var url = this.Url;
            var user = this.User;
            var viewBag = this.ViewBag;
            var viewData = this.ViewData;


            var action = this.ControllerContext.ActionDescriptor.ActionName;         // Index
            var controller = this.ControllerContext.ActionDescriptor.ControllerName; // Home

            // Note: you should avoid using HttpContext inside a controller
            var response = this.HttpContext.Response;
            //return this.BadRequest();
            return "Hello from Home Index";
        }
        // returning type ContentResult specifically allows you to send back text
        public ContentResult Index2()
        {
            return Content("Hello from Index2");
        }
        // returning type IActionResult allows you to return a variety of types of information
        public IActionResult Index3()
        {
            return Content("Hello from Index2");
        }
        public IActionResult Index4()
        {
            return this.BadRequest();
        }
    }
    #endregion

    #region HomeController3 (returning an object using ObjectModel)
    public class HomeController3 : Controller
    {
        public IActionResult Index()
        {
            Restaurant model = new Restaurant() { Id = 1, Name = "Francine's Place" };

            // this returns the object, serialized however was specified in the 
            // accept types header sent by the client to define what type of
            // serialization it can accept.  This is called "content negotiation".
            //
            //    {"id":1, "name":"Francine's Place"}  - converted to Json (by default)
            return new ObjectResult(model);
        }

    }
    #endregion

    #region HomeController4 (returning a simple View)
    public class HomeController4 : Controller
    {
        public IActionResult Index()
        {
            // if we just return this result without defining a view, we get an error.
            // There are two possible names for view that match this:
            //     /Views/Home/Index.cshtml
            //     /Views/Shared/Index.html
            return View();
        }
        public IActionResult Index2()
        {
            // if we specify the view's name to be home, these are the file names that match:
            //     /Views/Home/Home.cshtml
            //     /Views/Shared/Home.html
            return View("Home");
        }
    }
    #endregion

    #region HomeController5 (returning a view being passed a model)
    public class HomeController5 : Controller
    {
        public IActionResult Index3()
        {
            Restaurant restaurant = new Restaurant() { Name = "Francine's Burgers", Id = 1 };

            // This will look for /Views/Home/Index3.cshtml, which is a page expecting a
            // restaurant model to be passed to it.  See Example1.cshtml and Example2.cshtml
            // for examples of pages that will accept a restaurant model.
            return View(restaurant);
        }
    }
    #endregion

    #region HomeController6 (returning a view that takes a collection from a service)
    public class HomeController6 : Controller
    {
        // Note: in order to be able to pass this IRestaurantData into the method, it needs
        //       to be defined and registered.
        //
        //  The service is defined in /Services/IRestaurantData an /Services/InMemoryRestaurantData
        //
        //  Then the service must be registered in Startup.ConfigureServices:
        //       services.AddScoped<IRestaurantData, InMemoryRestaurantData>(); (see Startup8)
        //
        public HomeController6(IRestaurantData restaurantData)
        {
            _restaurantData = restaurantData;
        }

        // See Example3.cshtml for the view which accepts this model.
        public IActionResult Index()
        {
            return View(_restaurantData.GetAll());
        }
        private IRestaurantData _restaurantData;
    }
    #endregion

    #region HomeController7 (passing a ViewModel to a View)
    public class HomeController7 : Controller
    {
        // Note: in order to be able to pass this IRestaurantData into the method, it needs
        //       to be set up in Startup.ConfigureServices:
        //            services.AddScoped<IRestaurantData, InMemoryRestaurantData>();
        public HomeController7(IRestaurantData restaurantData, IGreeter greeter)
        {
            _restaurantData = restaurantData;
            _greeter = greeter;
        }

        public IActionResult Index()
        {
            var model = new HomeIndexViewModel();
            model.Restaurants = _restaurantData.GetAll();
            model.CurrentMessage = _greeter.GetMessageOfTheDay();

            return View(model);
        }
        private IRestaurantData _restaurantData;
        private IGreeter _greeter;
    }
    #endregion

    #region HomeController8 (add links to navigate between pages/endpoints)
    public class HomeController8 : Controller
    {
        // Note: in order to be able to pass this IRestaurantData into the method, it needs
        //       to be set up in Startup.ConfigureServices:
        //            services.AddScoped<IRestaurantData, InMemoryRestaurantData>();
        public HomeController8(IRestaurantData restaurantData, IGreeter greeter)
        {
            _restaurantData = restaurantData;
            _greeter = greeter;
        }

        public IActionResult Index()
        {
            var model = new HomeIndexViewModel();
            model.Restaurants = _restaurantData.GetAll();
            model.CurrentMessage = _greeter.GetMessageOfTheDay();

            return View(model);
        }

        // Because we have set up our route to take an "id" parameter, it knows to
        // parse this URL (localhost:xxxxx/home/details/22) and use it to call
        // the Details method with 22 passed in for the id
        //
        // Note: our routebuilder in Startup looks like this:
        //       routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");

        public IActionResult Details(int id)
        {
            var model = _restaurantData.Get(id);
            if (model == null)
            {
                // If the user has sent in an invalid id, redirect to the Index method
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        private IRestaurantData _restaurantData;
        private IGreeter _greeter;
    }
    #endregion

    #region HomeController9 (supports Create functionality)
    public class HomeController9 : Controller
    {
        // Note: in order to be able to pass this IRestaurantData into the method, it needs
        //       to be set up in Startup.ConfigureServices:
        //            services.AddScoped<IRestaurantData, InMemoryRestaurantData>();
        public HomeController9(IRestaurantData restaurantData, IGreeter greeter)
        {
            _restaurantData = restaurantData;
            _greeter = greeter;
        }

        public IActionResult Index()
        {
            var model = new HomeIndexViewModel();
            model.Restaurants = _restaurantData.GetAll();
            model.CurrentMessage = _greeter.GetMessageOfTheDay();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var model = _restaurantData.Get(id);
            if (model == null)
            {
                // If the user has sent in an invalid id, redirect to the Index method
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // This version of Create it to create the form and send it off to the client
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        // This version of create takes the posted value from the submitted Create page.
        // You could just pass it a Restaurant object, but because there are potentially
        // properties in Restaurant that we are not using, a hacker could add information
        // to the post.  This is called "overposting" - you get back more information than
        // you were expecting based on the form that you gave the user.  One simple way
        // to avoid this is to create a dedicated input model (example RestaurantEditModel)
        // that only includes whose elements that you expect from the form.
        [HttpPost]
        public IActionResult Create(RestaurantEditModel model)
        {
            Restaurant restaurant = new Restaurant();
            restaurant.Name = model.Name;
            restaurant.Cuisine = model.Cuisine;
            var newRestaurant = _restaurantData.Add(restaurant);

            return View("Details", newRestaurant);
        }

        private IRestaurantData _restaurantData;
        private IGreeter _greeter;
    }
    #endregion
}
