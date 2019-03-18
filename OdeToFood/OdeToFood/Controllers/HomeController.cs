using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OdeToFood.Models;
using OdeToFood.Services;

namespace OdeToFood.Controllers
{
    public class HomeController : Controller
    {
        // Note: in order to be able to pass this IRestaurantData into the method, it needs
        //       to be set up in Startup.ConfigureServices:
        //            services.AddScoped<IRestaurantData, InMemoryRestaurantData>();
        public HomeController(IRestaurantData restaurantData)
        {
            _restaurantData = restaurantData;
        }

        public IActionResult Index()
        {
            return View(_restaurantData.GetAll());
        }
        private IRestaurantData _restaurantData;
    }
}

