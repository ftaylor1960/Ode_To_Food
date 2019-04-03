using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OdeToFood.Data;
using OdeToFood.Models;

namespace OdeToFood.Services
{
    public class SqlRestaurantData : IRestaurantData
    {
        private OdeToFoodDbContext _context;

        public SqlRestaurantData(OdeToFoodDbContext context)
        {
            _context = context;
        }
        public Restaurant Add(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges(); // move this out to another method if you want to batch

            // after SaveChanges happens, the Id value will be generated and inserted into Id
            return restaurant;
        }

        public Restaurant Get(int id)
        {
            return _context.Restaurants.FirstOrDefault(r => r.Id == id);
        }

        // Note: for very large databases, like 10K+, you would want to return this as
        //       IQueryable instead of IEnumerable
        public IEnumerable<Restaurant> GetAll()
        {
            return _context.Restaurants.OrderBy(r => r.Name);
        }
    }
}
