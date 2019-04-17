using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OdeToFood.Services;

namespace OdeToFood.Pages
{
    public class GreetingModel : PageModel
    {
        private IGreeter _greeter;
        public string CurrentGreeting { get; set; }
        public string Name { get; set; }

        public GreetingModel(IGreeter greeter)
        {
            _greeter = greeter;
        }
        // This is like the Load method for a desktop form
        // The name parameter is specified in the @page on the razor page
        public void OnGet(string name)
        {
            CurrentGreeting = _greeter.GetMessageOfTheDay();
            Name = name;
        }
    }
}