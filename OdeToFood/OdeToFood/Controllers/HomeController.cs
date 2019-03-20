using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OdeToFood.Models;
using OdeToFood.Services;
using OdeToFood.ViewModels;

namespace OdeToFood.Controllers
{
    public class HomeController : Controller
    {
        // Note: in order to be able to pass this IRestaurantData into the method, it needs
        //       to be set up in Startup.ConfigureServices:
        //            services.AddScoped<IRestaurantData, InMemoryRestaurantData>();
        public HomeController(IRestaurantData restaurantData, IGreeter greeter)
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
        // Note: our routebuilder in Startrup looks like this:
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
}

