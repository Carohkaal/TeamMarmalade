using Newtonsoft.Json;
using System.Net.Sockets;

namespace Rooting.WebApi.Client.UnitTests
{
    [TestClass]
    public class TestBasicClient
    {
        [TestMethod]
        public void ClientCanInitialize()
        {
            var c = new UseClient();
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public async Task ClientCanReadData()
        {
            var c = new UseClient();
            var data = await c.CurrentForecastAsync();
            Assert.IsNotNull(data);
            Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}