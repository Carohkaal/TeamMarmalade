namespace Rooting.WebApi.Client
{
    public class UseClient
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly RootingWebApiClient _apiClient;

        public UseClient()
        {
            _httpClient.BaseAddress = new Uri("https://rootingwebapi.azurewebsites.net");
            _apiClient = new RootingWebApiClient(_httpClient);
        }

        public async Task<PlayerModel[]> CurrentPlayersAsync()
        {
            var fc = await _apiClient.CurrentPlayersAsync();
            return fc.ToArray();
        }
    }
}