using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// This allows you to add the Data Annotations.
using System.ComponentModel.DataAnnotations;

namespace OdeToFood.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Display(Name="Restaurant Name")]
        [DataType(DataType.Password)]  // This would cause the test to be hidden as it is being typed
        [Required, MaxLength(80)]
        public string Name { get; set; }

        public CuisineType Cuisine { get; set; }
    }
}
