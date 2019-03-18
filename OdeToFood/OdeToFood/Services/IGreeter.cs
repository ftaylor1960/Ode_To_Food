using Microsoft.Extensions.Configuration;

namespace OdeToFood.Services
{
    public interface IGreeter
    {
        string GetMessageOfTheDay();
    }
    public class Greeter : IGreeter
    {
        string _configMessage = "Greetings";
        public Greeter(IConfiguration configuration)
        {
            string greeting = configuration["Greeting"];
            if (!string.IsNullOrWhiteSpace(greeting))
            {
                _configMessage = greeting;
            }
        }
        public string GetMessageOfTheDay()
        {
            return _configMessage;
        }
    }
}