using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OdeToFood.Models;

namespace OdeToFood.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // if we just return this result without defining a view, we get an error.
            // There are two possible names for view that match this:
            //     /Views/Home/Index.cshtml
            //     /Views/Shared/Index.html
            return View();
        }

    }
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
}

