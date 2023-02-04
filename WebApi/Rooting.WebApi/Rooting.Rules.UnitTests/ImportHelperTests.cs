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
            var m = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            var importResult = await ImportHelper.ImportSetup(m);
            Assert.IsNotNull(importResult);
            Assert.IsNotNull(importResult.Requirements);
            Assert.IsNotNull(importResult.Actions);
            Assert.IsNotNull(importResult.Cards);
            Assert.IsNotNull(importResult.Deck);
        }

        //SampleData

        [TestMethod]
        public async Task ImportHelperCanLoadJson()
        {
            var m = new MemoryStream(Encoding.UTF8.GetBytes(SampleData));
            var importResult = await ImportHelper.ImportSetup(m);
            Assert.IsNotNull(importResult);
            Assert.AreEqual(3, importResult.Requirements.Count);
            Assert.AreEqual(2, importResult.Actions.Count);
            Assert.AreEqual(2, importResult.Cards.Count);
            Assert.AreEqual(5, importResult.Deck.Count);
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
                Name = "Bar-Action",
                Scores = new Score[]
    {
                    new Score { FamilyTypes= FamilyTypes.Animal, ScoreType = ScoreType.Mission, ScoreValue = -1 },
                    new Score { FamilyTypes= FamilyTypes.Fungi, ScoreType = ScoreType.Capture, ScoreValue = 5 },
                    new Score { FamilyTypes= FamilyTypes.Tree, ScoreType = ScoreType.Distance, ScoreValue = 2 },
    }
            };
            model.Actions.Add("BAR-ACTION", act2);

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

            model.Deck.Add(1, pawn.AsNewPlayingCard(1));
            model.Deck.Add(2, pawn.AsNewPlayingCard(2));
            model.Deck.Add(3, pawn.AsNewPlayingCard(3));
            model.Deck.Add(4, rook.AsNewPlayingCard(4));
            model.Deck.Add(5, rook.AsNewPlayingCard(5));

            model.DefineMap(
                "___########___" + Environment.NewLine +
                "__##T#######__" + Environment.NewLine +
                "_######A###___" + Environment.NewLine +
                "____#F#######_" + Environment.NewLine +
                "___########___" + Environment.NewLine +
                "____######____");

            await ImportHelper.WriteExport(model, "c:\\tmp\\Game-Setup.json");

            Assert.IsTrue(true, "Export completed");
        }

        private const string SampleData = @"{
  ""Requirements"": [
    {
      ""Name"": ""Foo"",
      ""Description"": ""Foo description"",
      ""RequireTier"": 1,
      ""RequireFamily"": ""Tree"",
      ""RequireTileControl"": true,
      ""RequireTileDistance"": 1
    },
    {
      ""Name"": ""Bar"",
      ""Description"": ""Bar description"",
      ""RequireTier"": 0,
      ""RequireFamily"": ""Fungi"",
      ""RequireTileControl"": false,
      ""RequireTileDistance"": 0
    },
    {
      ""Name"": ""Alice"",
      ""Description"": ""Alice description"",
      ""RequireTier"": 0,
      ""RequireFamily"": ""Animal"",
      ""RequireTileControl"": false,
      ""RequireTileDistance"": 0
    }
  ],
  ""Actions"": [
    {
      ""Name"": ""Foo-Action"",
      ""Cost"": 1,
      ""Scores"": [
        {
          ""FamilyType"": ""Animal"",
          ""ScoreType"": ""Mission"",
          ""Score"": 2
        },
        {
          ""FamilyType"": ""Fungi"",
          ""ScoreType"": ""Capture"",
          ""Score"": -2
        },
        {
          ""FamilyType"": ""Tree"",
          ""ScoreType"": ""Distance"",
          ""Score"": 5
        }
      ]
    },
    {
      ""Name"": ""Bar-Action"",
      ""Cost"": 1,
      ""Scores"": [
        {
          ""FamilyType"": ""Animal"",
          ""ScoreType"": ""Mission"",
          ""Score"": 2
        },
        {
          ""FamilyType"": ""Fungi"",
          ""ScoreType"": ""Capture"",
          ""Score"": -2
        },
        {
          ""FamilyType"": ""Tree"",
          ""ScoreType"": ""Distance"",
          ""Score"": 5
        }
      ]
    }
  ],
  ""Cards"": [
    {
      ""Name"": ""Pawn"",
      ""Tier"": 1,
      ""FamilyType"": ""Fungi"",
      ""Description"": ""Pawn for fungi"",
      ""Art"": ""https://rootingwebapi.azurewebsites.net/art/pawn.jpg"",
      ""PlayRange"": 1,
      ""Actions"": [
        ""Foo-Action""
      ],
      ""Requirements"": [
        ""Foo"",
        ""Bar""
      ]
    },
    {
      ""Name"": ""Rook"",
      ""Tier"": 2,
      ""FamilyType"": ""Fungi"",
      ""Description"": ""Rook for fungi"",
      ""Art"": ""https://rootingwebapi.azurewebsites.net/art/pawn.jpg"",
      ""PlayRange"": 3,
      ""Actions"": [
        ""Foo-Action"",
        ""Bar-Action""
      ],
      ""Requirements"": [
        ""Alice""
      ]
    }
  ],
  ""Deck"": [
    {
      ""Id"": 1,
      ""Name"": ""Pawn""
    },
    {
      ""Id"": 2,
      ""Name"": ""Pawn""
    },
    {
      ""Id"": 3,
      ""Name"": ""Pawn""
    },
    {
      ""Id"": 4,
      ""Name"": ""Rook""
    },
    {
      ""Id"": 5,
      ""Name"": ""Rook""
    }
  ],
  ""Map"": [
    ""############"",
    ""..##T#######"",
    "".##T########"",
    ""....#F######"",
    ""##T########.""
  ]
}";
    }
}