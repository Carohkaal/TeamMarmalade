using Newtonsoft.Json;
using Rooting.Models;
using Rooting.Models.ImportExport;

namespace Rooting.Rules
{
    public static class ImportHelper
    {
        public static async Task<GameImport?> ReadGameImport(Stream s)
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
            };

            var data = JsonConvert.SerializeObject(e);
            using var sw = new StreamWriter(path);
            await sw.WriteAsync(data);
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

        public static void ImportDeck(GameSetup setup, GameImport m)
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
                    setup.Deck.Add(item.Id, card);
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
                {
                    setup.Actions.Remove(name);
                }
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
                {
                    setup.Requirements.Remove(name);
                }
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
                {
                    setup.Cards.Remove(name);
                }
                setup.Cards.Add(name, item);
            }
        }

        private static CardBase? InterpretCard(GameSetup setup, DefineCard? data)
        {
            if (data == null) return null;
            var c = new CardBase
            {
                Name = data.Name,
                Description = data.Description,
                Art = data.Art,
                Tier = data.Tier,
                PlayRange = data.PlayRange,
            };

            var cost = 0;
            foreach (var a in data.Actions)
            {
                var action = setup.Actions[a.ToUpperInvariant()];
                if (action == null)
                {
                    setup.NotFound(nameof(ActionBase), $"Could not find action {a}");
                    continue;
                }
                c.Actions.Add(action);
                cost += action.Cost;
            }
            c.TotalCost = cost;

            foreach (var r in data.Requirements)
            {
                var requirement = setup.Requirements[r.ToUpperInvariant()];
                if (requirement == null)
                {
                    setup.NotFound(nameof(Requirement), $"Could not find requirement {r}");
                    continue;
                }
                c.Requirements.Add(requirement);
            }

            return c;
        }
    }
}