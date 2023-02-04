using Rooting.Models;
using System.Text;

namespace Rooting.Rules.UnitTests
{
    [TestClass]
    public class ImportHelperTests
    {
        [TestMethod]
        public async Task ImportHelperCanLoad()
        {
            var m = new MemoryStream(Encoding.UTF8.GetBytes("[]"));
            var importResult = await ImportHelper.ReadGameImport(m);
            Assert.IsNotNull(importResult);
            Assert.IsNotNull(importResult.Requirements);
            Assert.IsNotNull(importResult.Actions);
            Assert.IsNotNull(importResult.Cards);
        }

        [TestMethod]
        public async Task ImportHelperCanExport()
        {
            var model = new GameSetup();

            var foo = new Requirement
            {
                Description = "Foo description",
                Name = "Foo",
                RequireFamily = FamilyTypes.Tree,
                RequireTier = 1,
                RequireTileControl = true,
                RequireTileDistance = 1,
            };

            var bar = new Requirement
            {
                Description = "Bar description",
                Name = "Bar",
                RequireFamily = FamilyTypes.Fungi,
            };

            var alice = new Requirement
            {
                Description = "Alice description",
                Name = "Alice",
                RequireFamily = FamilyTypes.Animal,
            };

            model.Requirements.Add("FOO", foo);
            model.Requirements.Add("BAR", bar);
            model.Requirements.Add("ALICE", alice);

            var act1 = new ActionBase
            {
                Cost = 1,
                Name = "Foo-Action",
                Scores = new Score[]
                {
                    new Score { FamilyTypes= FamilyTypes.Animal, ScoreType = ScoreType.Mission, ScoreValue = 2 },
                    new Score { FamilyTypes= FamilyTypes.Fungi, ScoreType = ScoreType.Capture, ScoreValue = -2 },
                    new Score { FamilyTypes= FamilyTypes.Tree, ScoreType = ScoreType.Distance, ScoreValue = 5 },
                }
            };
            model.Actions.Add("FOO-ACTION", act1);

            var act2 = new ActionBase
            {
                Cost = 2,
                Name = "Foo-Action",
                Scores = new Score[]
    {
                    new Score { FamilyTypes= FamilyTypes.Animal, ScoreType = ScoreType.Mission, ScoreValue = -1 },
                    new Score { FamilyTypes= FamilyTypes.Fungi, ScoreType = ScoreType.Capture, ScoreValue = 5 },
                    new Score { FamilyTypes= FamilyTypes.Tree, ScoreType = ScoreType.Distance, ScoreValue = 2 },
    }
            };
            model.Actions.Add("FOO-ACTION", act1);

            var pawn = new CardBase
            {
                Actions = new List<ActionBase> { act1 },
                Art = "https://rootingwebapi.azurewebsites.net/art/pawn.jpg",
                Description = "Pawn for fungi",
                FamilyTypes = FamilyTypes.Fungi,
                Name = "Pawn",
                PlayRange = 1,
                Requirements = new List<Requirement> { foo, bar },
                Tier = 1,
                TotalCost = 1,
            };
            model.Cards.Add("PAWN", pawn);

            var rook = new CardBase
            {
                Actions = new List<ActionBase> { act1, act2 },
                Art = "https://rootingwebapi.azurewebsites.net/art/pawn.jpg",
                Description = "Rook for fungi",
                FamilyTypes = FamilyTypes.Fungi,
                Name = "Rook",
                PlayRange = 3,
                Requirements = new List<Requirement> { alice },
                Tier = 2,
                TotalCost = 3,
            };
            model.Cards.Add("ROOK", rook);

            model.Deck.Add(1, pawn);
            model.Deck.Add(2, pawn);
            model.Deck.Add(3, pawn);
            model.Deck.Add(4, rook);
            model.Deck.Add(5, rook);

            await ImportHelper.WriteExport(model, "c:\\tmp\\Game-Setup.json");

            Assert.IsTrue(true, "Export completed");
        }
    }
}