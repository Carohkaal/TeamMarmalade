using Rooting.WebApi.Client.Api;
using Rooting.WebApi.Client.Model;

namespace Rooting.WebApi.Client
{
    public class UseClient
    {
        private readonly WeatherForecastApi _apiClient = new WeatherForecastApi("https://rootingwebapi.azurewebsites.net/");

        public UseClient()
        {
        }

        public async Task<WeatherForecast[]> CurrentForecastAsync()
        {
            var fc = await _apiClient.GetWeatherForecastAsync();
            return fc.ToArray();
        }
    }
}