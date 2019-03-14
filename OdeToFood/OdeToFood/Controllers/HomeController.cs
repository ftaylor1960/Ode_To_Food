using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdeToFood.Controllers
{
    // The name of this controller is significant.  MVC uses HomeController as its default,
    // so if the request does not ask for a specific controller, MVC will try to route to
    // the Home controller.
    public class HomeController
    {
        // Index is the default method in a controller.  If MVC knows what controller to
        // invoke but doesn't have a a specificmethod, it will try to call the Index method
        public string Index()
        {
            // we can just return a simple string and it will show up on a web page
            return "Hello from the home controller";
        }
    }
}
