using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OdeToFood.Controllers
{
    // this tells MVC that this controller is called when /about is the first part of the URL,
    // because the controller name is "about"
    [Route("[controller]")]
    public class AboutController
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
}