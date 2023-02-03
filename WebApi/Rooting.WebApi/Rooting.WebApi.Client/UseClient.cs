using Rooting.Api;

namespace Rooting.WebApi.Client
{
    public class UseClient
    {
        private readonly HttpClient _httpClient = new();

        public UseClient()
        {
        }

        public async Task<WeatherForecast[]> CurrentForecastAsync()
        {
            var c = new RootingClient("https://rootingwebapi.azurewebsites.net/", _httpClient);
            var fc = await c.GetWeatherForecastAsync();
            return fc.ToArray();
        }
    }
}