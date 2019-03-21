using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OdeToFood.Models;

namespace OdeToFood.Services
{
    public class InMemoryRestaurantData : IRestaurantData
    {
        public InMemoryRestaurantData()
        {
            _restaurants = new List<Restaurant>();
            _restaurants.Add(new Restaurant() { Id = 1, Name = "Budd's Pizza", Cuisine=CuisineType.none });
            _restaurants.Add(new Restaurant() { Id = 2, Name = "Maggie's Soda Shoppe" });
            _restaurants.Add(new Restaurant() { Id = 3, Name = "Elvis Burgers and Booze" });
            _restaurants.Add(new Restaurant() { Id = 4, Name = "Nin Hao Happiness Chinese Food", Cuisine=CuisineType.Chinese });
            _restaurants.Add(new Restaurant() { Id = 5, Name = "Guido's Authentic Italian", Cuisine = CuisineType.Italian});
        }

        public IEnumerable<Restaurant> GetAll()
        {
            return _restaurants;
        }

        public Restaurant Get(int id)
        {
            return _restaurants.FirstOrDefault(r => r.Id == id);
        }
        public Restaurant Add(Restaurant restaurant)
        {
            restaurant.Id = _restaurants.Count + 1;
            _restaurants.Add(restaurant);
            return restaurant;
        }
        List<Restaurant> _restaurants;
    }
}
