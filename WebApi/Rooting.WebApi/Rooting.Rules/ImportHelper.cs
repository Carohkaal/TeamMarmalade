using Newtonsoft.Json;
using Rooting.Models;
using Rooting.Models.ImportExport;

namespace Rooting.Rules
{
    public static class ImportHelper
    {
        public static char ToChar(this FamilyTypes value)
        {
            switch (value)
            {
                case FamilyTypes.Animal: return 'A';
                case FamilyTypes.Plant: return 'T';
                case FamilyTypes.Fungi: return 'F';
                case FamilyTypes.All: return '#';
                default: return '.';
            }
        }

        public static FamilyTypes ToFamilyType(this char c)
        {
            switch (char.ToUpperInvariant(c))
            {
                case 'A': return FamilyTypes.Animal;
                case 'T': return FamilyTypes.Plant;
                case 'F': return FamilyTypes.Fungi;
                case '#': return FamilyTypes.All;
                default: return FamilyTypes.Any;
            }
        }

        private static async Task<GameImport?> ReadGameImport(Stream s)
        {
            var ts = new StreamReader(s);
            var content = await ts.ReadToEndAsync();
            return JsonConvert.DeserializeObject<GameImport>(content);
        }

        public static async Task WriteExport(GameSetup setup, string path)
        {
            var e = new GameImport
            {
                Requirements = ExtractRequirements(setup),
                Actions = ExtractActions(setup),
                Cards = ExtractCards(setup),
                Deck = ExtractDeck(setup),
                Map = ExtractMap(setup)
            };

            var data = JsonConvert.SerializeObject(e, Formatting.Indented);
            using var sw = new StreamWriter(path);
            await sw.WriteAsync(data);
        }

        private static IEnumerable<string> ExtractMap(GameSetup setup)
        {
            var rows = 0;
            var cols = 0;
            foreach (var card in setup.Map)
            {
                if (card.Row > rows) rows = card.Row;
                if (card.Col > cols) cols = card.Col;
            }
            for (var y = 0; y < rows; y++)
            {
                var rowString = new char[cols];
                for (var x = 0; x < cols; x++)
                {
                    var type = setup.TileBaseType(y, x);
                    rowString[x] = type.ToChar();
                }
                yield return new string(rowString);
            }
        }

        public static PlayingCard AsNewPlayingCard(this CardBase item, int id)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return new PlayingCard
            {
                Id = id,
                FamilyType = item.FamilyTypes,
                Name = item.Name,
                PlayingState = PlayingState.InStock
            };
        }

        private static List<DefineDeck> ExtractDeck(GameSetup setup)
        {
            return setup.Deck
                .Select(m => new DefineDeck
                {
                    Id = m.Key,
                    Name = m.Value.Name
                }).ToList();
        }

        private static List<DefineCard> ExtractCards(GameSetup setup)
        {
            var ec = new List<DefineCard>();
            foreach (var kv in setup.Cards)
            {
                var item = kv.Value;
                var card = new DefineCard
                {
                    Actions = item.Actions.Select(x => x.Name).ToArray(),
                    Art = item.Art,
                    Description = item.Description,
                    FamilyType = item.FamilyTypes.ToString(),
                    Name = item.Name,
                    PlayRange = item.PlayRange,
                    Requirements = item.Requirements.Select(x => x.Name).ToArray(),
                    Tier = item.Tier,
                };
                ec.Add(card);
            }

            return ec;
        }

        private static List<DefineAction> ExtractActions(GameSetup setup)
        {
            var ea = new List<DefineAction>();
            foreach (var kv in setup.Actions)
            {
                var item = kv.Value;
                var model = new DefineAction
                {
                    Name = item.Name,
                    Cost = item.Cost,
                };

                var esc = new List<DefineFamilyScore>();
                foreach (var sc in item.Scores)
                {
                    var scoreModel = new DefineFamilyScore
                    {
                        Score = sc.ScoreValue,
                        FamilyType = sc.FamilyTypes.ToString(),
                        ScoreType = sc.ScoreType.ToString()
                    };
                    esc.Add(scoreModel);
                }
                model.Scores = esc.ToArray();
                ea.Add(model);
            }

            return ea;
        }

        private static List<DefineRequirement> ExtractRequirements(GameSetup setup)
        {
            var er = new List<DefineRequirement>();
            foreach (var kv in setup.Requirements)
            {
                var item = kv.Value;
                var model = new DefineRequirement
                {
                    Description = item.Description,
                    Name = item.Name,
                    RequireFamily = item.RequireFamily.ToString(),
                    RequireTier = item.RequireTier,
                    RequireTileControl = item.RequireTileControl,
                    RequireTileDistance = item.RequireTileDistance
                };
                er.Add(model);
            }

            return er;
        }

        public static async Task<GameSetup> ImportSetup(Stream? importStream)
        {
            var setup = new GameSetup();
            if (importStream == null)
            {
                return setup;
            }
            var importFile = await ReadGameImport(importStream);
            if (importFile == null) return setup;

            ImportActions(setup, importFile);
            ImportRequirements(setup, importFile);
            ImportCards(setup, importFile);
            ImportDeck(setup, importFile);
            ImportMap(setup, importFile);

            return setup;
        }

        private static void ImportMap(GameSetup setup, GameImport m)
        {
            var row = 0;
            foreach (var item in m.Map)
            {
                var col = 0;
                if (item == null) continue;
                foreach (var c in item)
                {
                    var fam = c.ToFamilyType();
                    if (fam != FamilyTypes.Any)
                    {
                        setup.AddTile(row, col, fam);
                    }
                    col++;
                }
                row++;
            }
        }

        private static void ImportDeck(GameSetup setup, GameImport m)
        {
            foreach (var item in m.Deck)
            {
                if (item == null) continue;

                var name = item.Name.ToUpperInvariant();
                if (!setup.Cards.ContainsKey(name))
                {
                    setup.Invalid(nameof(DefineDeck), $"Invalid card in deck {item.Name}");
                }
                else
                {
                    var card = setup.Cards[name];
                    var playingCard = new PlayingCard
                    {
                        Id = item.Id,
                        Name = card.Name,
                        FamilyType = card.FamilyTypes,
                        PlayingState = PlayingState.InStock
                    };
                    setup.Deck.Add(item.Id, playingCard);
                }
            }
        }

        public static void ImportActions(GameSetup setup, GameImport m)
        {
            foreach (var a in m.Actions)
            {
                var item = InterpretAction(setup, a);
                if (item == null) continue;

                var name = item.Name.ToUpperInvariant();
                if (setup.Actions.ContainsKey(name))
                    setup.Invalid(nameof(ActionBase), $"Duplicate key in actions {name}");
                else
                    setup.Actions.Add(name, item);
            }
        }

        private static ActionBase? InterpretAction(GameSetup setup, DefineAction? data)
        {
            if (data == null) return null;
            var r = new ActionBase
            {
                Cost = data.Cost,
                Name = data.Name
            };
            var scores = new List<Score>();
            foreach (var s in data.Scores)
            {
                if (!Enum.TryParse<FamilyTypes>(s.FamilyType, out var fam))
                {
                    setup.Invalid(nameof(FamilyTypes), $"Invalid family in action {data.Name} : {s.FamilyType}, should be one of: {Enum.GetNames<FamilyTypes>()}");
                    return null;
                }
                if (!Enum.TryParse<ScoreType>(s.ScoreType, out var score))
                {
                    setup.Invalid(nameof(ScoreType), $"Invalid scoretype in action {data.Name} : {s.ScoreType}, should be one of: {Enum.GetNames<ScoreType>()}");
                    return null;
                }
                scores.Add(new Score
                {
                    FamilyTypes = fam,
                    ScoreType = score,
                    ScoreValue = s.Score
                });
            }
            r.Scores = scores;
            return r;
        }

        public static void ImportRequirements(GameSetup setup, GameImport m)
        {
            foreach (var a in m.Requirements)
            {
                var item = InterpretRequirement(setup, a);
                if (item == null) continue;

                var name = item.Name.ToUpperInvariant();
                if (setup.Requirements.ContainsKey(name))
                    setup.Invalid(nameof(Requirement), $"Duplicate key in requirements {name}");
                else
                    setup.Requirements.Add(name, item);
            }
        }

        private static Requirement? InterpretRequirement(GameSetup setup, DefineRequirement? data)
        {
            if (data == null) return null;
            var r = new Requirement
            {
                Name = data.Name,
                Description = data.Description,
                RequireTier = data.RequireTier,
                RequireTileControl = data.RequireTileControl,
                RequireTileDistance = data.RequireTileDistance,
            };

            if (!Enum.TryParse<FamilyTypes>(data.RequireFamily, out var fam))
            {
                setup.Invalid(nameof(FamilyTypes), $"Invalid family in requirement {data.Name} : {data.RequireFamily}, should be one of: {Enum.GetNames<FamilyTypes>()}");
                return null;
            }
            r.RequireFamily = fam;
            return r;
        }

        public static void ImportCards(GameSetup setup, GameImport m)
        {
            foreach (var card in m.Cards)
            {
                var item = InterpretCard(setup, card);
                if (item == null) continue;

                var name = item.Name.ToUpperInvariant();
                if (setup.Cards.ContainsKey(name))
                    setup.Invalid(nameof(FamilyTypes), $"Duplicate key in cards {name}");
                else
                    setup.Cards.Add(name, item);
            }
        }

        private static CardBase? InterpretCard(GameSetup setup, DefineCard? data)
        {
            if (data == null) return null;

            if (!Enum.TryParse<FamilyTypes>(data.FamilyType, out var type))
            {
                setup.NotFound(nameof(CardBase), $"Could not find family {data.FamilyType} for card {data.Name}");
            }

            var c = new CardBase
            {
                Name = data.Name,
                Description = data.Description,
                Art = data.Art,
                Tier = data.Tier,
                Score = data.Score,
                PlayRange = data.PlayRange,
                FamilyTypes = type,
            };

            var cost = 0;
            var actions = new List<ActionBase>();
            foreach (var a in data.Actions)
            {
                var action = setup.Actions[a.ToUpperInvariant()];
                if (action == null)
                {
                    setup.NotFound(nameof(ActionBase), $"Could not find action {a}");
                    continue;
                }
                actions.Add(action);
                cost += action.Cost;
            }
            c.Actions = actions;
            c.TotalCost = cost;

            var requirements = new List<Requirement>();
            foreach (var r in data.Requirements)
            {
                var requirement = setup.Requirements[r.ToUpperInvariant()];
                if (requirement == null)
                {
                    setup.NotFound(nameof(Requirement), $"Could not find requirement {r}");
                    continue;
                }
                requirements.Add(requirement);
            }
            c.Requirements = requirements;

            return c;
        }
    }
}