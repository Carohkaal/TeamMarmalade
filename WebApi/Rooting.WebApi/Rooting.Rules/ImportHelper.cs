using Newtonsoft.Json;
using Rooting.Models;
using Rooting.Models.ImportExport;

namespace Rooting.Rules
{
    public class ImportHelper
    {
        public async Task<GameImport?> ReadGameImport(Stream s)
        {
            var ts = new StreamReader(s);
            var content = await ts.ReadToEndAsync();
            return JsonConvert.DeserializeObject<GameImport>(content);
        }

        public void ImportActions(GameSetup setup, GameImport m)
        {
            foreach (var a in m.Actions)
            {
                var item = InterpretAction(setup, a);
                if (item == null) continue;

                var name = item.Name;
                if (setup.Actions.ContainsKey(name))
                {
                    setup.Actions.Remove(name);
                }
                setup.Actions.Add(name, item);
            }
        }

        private ActionBase? InterpretAction(GameSetup setup, DefineAction? data)
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

        public void ImportRequirements(GameSetup setup, GameImport m)
        {
            foreach (var a in m.Requirements)
            {
                var item = InterpretRequirement(setup, a);
                if (item == null) continue;

                var name = item.Name;
                if (setup.Requirements.ContainsKey(name))
                {
                    setup.Requirements.Remove(name);
                }
                setup.Requirements.Add(name, item);
            }
        }

        private Requirement? InterpretRequirement(GameSetup setup, DefineRequirement? data)
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

        public void ImportCards(GameSetup setup, GameImport m)
        {
            foreach (var card in m.Cards)
            {
                var item = InterpretCard(setup, card);
                if (item == null) continue;

                var name = item.Name;
                if (setup.Cards.ContainsKey(name))
                {
                    setup.Cards.Remove(name);
                }
                setup.Cards.Add(name, item);
            }
        }

        private CardBase? InterpretCard(GameSetup setup, DefineCard? data)
        {
            if (data == null) return null;
            var c = new CardBase
            {
                Name = data.Name,
                Description = data.Description,
                Art = data.Art,
                Tier = data.Tier,
                PlayRange = data.PlayRange,
                TotalCost = data.TotalCost
            };

            foreach (var a in data.Actions)
            {
                var action = setup.Actions[a];
                if (action == null)
                {
                    setup.NotFound(nameof(ActionBase), $"Could not find action {a}");
                    continue;
                }
                c.Actions.Add(action);
            }

            foreach (var r in data.Requirements)
            {
                var requirement = setup.Requirements[r];
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