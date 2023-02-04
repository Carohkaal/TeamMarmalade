using Rooting.WebApi.Client.Api;
using Rooting.WebApi.Client.Model;

namespace Rooting.WebApi.Client
{
    public class UseClient
    {
        private readonly PlayerApi _apiClient = new PlayerApi("https://rootingwebapi.azurewebsites.net/");

        public UseClient()
        {
        }

        public async Task<Player[]> CurrentPlayersAsync()
        {
            var fc = await _apiClient.PlayerCurrentPlayersGetAsync();
            return fc.ToArray();
        }
    }
}