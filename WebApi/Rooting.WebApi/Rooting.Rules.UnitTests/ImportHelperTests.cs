using System.Text;
using System.Text.Unicode;

namespace Rooting.Rules.UnitTests
{
    [TestClass]
    public class ImportHelperTests
    {
        [TestMethod]
        public void ImportHelperCanInitialize()
        {
            var helper = new ImportHelper();
            Assert.IsNotNull(helper);
        }

        [DataTestMethod]
        public async Task ImportHelperCanLoad()
        {
            var m = new MemoryStream(Encoding.UTF8.GetBytes("[]"));
            var helper = new ImportHelper();
            var importResult = await helper.ReadGameImport(m);
            Assert.IsNotNull(importResult);
            Assert.IsNotNull(importResult.Requirements);
            Assert.IsNotNull(importResult.Actions);
            Assert.IsNotNull(importResult.Cards);
        }
    }
}