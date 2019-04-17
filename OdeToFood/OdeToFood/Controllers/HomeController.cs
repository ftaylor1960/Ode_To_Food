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
        //[ValidateAntiForgeryToken]  // This should be used for any POST type controller
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
        //[ValidateAntiForgeryToken]  // This should be used for any POST type controller
        public IActionResult Create(RestaurantEditModel model)
        {
            // ModelState tells you a lot of information about the state of teh model.  This information
            // is gathered based on the attributes placed on the ViewModel.
            if (ModelState.IsValid)
            {
                // This is called "model binding", binding data into your model
                Restaurant restaurant = new Restaurant();
                restaurant.Name = model.Name;
                restaurant.Cuisine = model.Cuisine;
                var newRestaurant = _restaurantData.Add(restaurant);

                // If we return a view and the user refreshes the page, it will call Create again,
                // and we might end up with a duplicate POST.  Instead, we will redirect to the
                // Details page and tell it to get the restaurant that we just created by Id.

                //return View("Details", newRestaurant);
                return RedirectToAction(nameof(Details), new { id = newRestaurant.Id });
            }
            else
            {
                return View();
            }
        }

        private IRestaurantData _restaurantData;
        private IGreeter _greeter;
    }
}

