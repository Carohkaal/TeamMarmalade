using Rooting.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rooting.Models.ResponseModels;
using Newtonsoft.Json.Linq;

namespace Rooting.Rules
{
    internal static class GameRuleExtensions
    {
        public static IEnumerable<IScoringToken> AddToken(this IEnumerable<IScoringToken> tokens, Func<IScoringToken> tokenFunction)
        {
            var newTokens = new List<IScoringToken>();
            newTokens.AddRange(tokens);
            newTokens.Add(tokenFunction.Invoke());
            return newTokens;
        }

        public static IEnumerable<IScoringToken> Remove(this IEnumerable<IScoringToken> tokens, Func<IScoringToken, bool> remove)
        {
            var newTokens = new List<IScoringToken>();
            foreach (var item in tokens)
            {
                if (item == null) continue;
                if (remove.Invoke(item)) continue;
                newTokens.Add(item);
            }
            return newTokens;
        }

        public static int ScoresDifference(this TileBase tile, FamilyTypes family)
        {
            var familyScore = tile.ScoringClass.FamilyScore.Where(m => m.Key == family).Sum(m => m.Value);
            var otherScore = tile.ScoringClass.FamilyScore.Where(m => m.Key != family).Sum(m => m.Value);
            return familyScore - otherScore;
        }

        public static TileBase SubScore(this TileBase tile, FamilyTypes family, int score)
        {
            if (score == 0) return tile;
            tile.ScoringClass.AddSubScore(family, score);
            return tile;
        }

        public static TileBase Score(this TileBase tile, FamilyTypes family, int score)
        {
            if (score == 0) return tile;
            tile.ScoringClass.AddScore(family, score);
            return tile;
        }

        public static TileBase Score(this TileBase tile, FamilyTypes family, Func<int, int> calculate)
        {
            var score = tile.ScoringClass.FamilyScore[family];
            if (score == 0) return tile;

            var newScore = calculate(score);
            tile.ScoringClass.SetScore(family, newScore);
            return tile;
        }

        public static int FamilyScore(this TileBase tile, FamilyTypes family)
        {
            return tile.ScoringClass.FamilyScore.Where(m => m.Key == family).Sum(m => m.Value);
        }

        public static bool FamilyRules(this TileBase tile, FamilyTypes family)
        {
            var myScore = FamilyScore(tile, family);
            var otherScore = 0;
            foreach (FamilyTypes fam in Enum.GetValues(typeof(FamilyTypes)))
            {
                if (fam == family) continue;
                var fs = FamilyScore(tile, fam);
                if (fs > otherScore) otherScore = fs;
            }
            return (myScore > otherScore);
        }

        public static int OtherFamilyScore(this TileBase tile, FamilyTypes family)
        {
            return tile.ScoringClass.FamilyScore.Where(m => m.Key != family).Sum(m => m.Value);
        }

        public static void AddSettlement(this TileBase? tile, FamilyTypes family, TokenType tokenType)
        {
            if (tile == null) return;
            var newTokens = tile.ScoringClass.Tokens
                    .AddToken(() => ScoringToken.CreateToken(family, tokenType));
            tile.ScoringClass.UpdateTokens(newTokens);
        }

        public static void SettlementUpgrade(this TileBase? tile, FamilyTypes family)
        {
            if (tile == null) return;
            var scoringTokens = tile.ScoringClass.Tokens.Where(m => m.Family == family);
            if (scoringTokens.Count() == 0) return;

            if (scoringTokens.Any(m => m.Level == TokenType.Village))
            {
                if (tile.ScoresDifference(family) > 15)
                {
                    var newTokens = tile.ScoringClass.Tokens
                        .Remove((m) => m.Level == TokenType.Village && m.Family == family)
                        .AddToken(() => ScoringToken.CreateToken(family, TokenType.City));
                    tile.ScoringClass.UpdateTokens(newTokens);
                }
                return;
            }

            if (tile.ScoresDifference(family) > 10)
            {
                var newTokens = tile.ScoringClass.Tokens
                    .AddToken(() => ScoringToken.CreateToken(family, TokenType.Village));
                tile.ScoringClass.UpdateTokens(newTokens);
            }
        }
    }

    public class GameRulesGGJ2023 : IGameEngine
    {
        private readonly ILogger<GameRulesGGJ2023> logger;

        private readonly Random r = new Random();

        private readonly Dictionary<string, Action<Origin, WorldMap>> TileRules = new()
        {
            { "PLANTRULECOLONIZATION", (o, m) => {
                var tile = m.Tile(o);
                if (tile==null) return;
                tile.SettlementUpgrade(FamilyTypes.Plant);

                if( tile.OtherFamilyScore(FamilyTypes.Plant)>0)
                    tile.Score(FamilyTypes.Plant, 2);
                else
                    tile.Score(FamilyTypes.Plant, 10);
               }
            },
            { "PLANTRULEASSIMILATE", (o, m) => {
                var tile = m.Tile(o);
                if (tile==null) return;

                if( tile.OtherFamilyScore(FamilyTypes.Plant)>0)
                    tile.Score(FamilyTypes.Plant, 6);
                else
                    tile.Score(FamilyTypes.Plant, 2);
            } },
            { "PLANTRULEROOTING", (o, m) => {
                var tile = m.Tile(o);
                if (tile==null) return;

                if (tile.FamilyRules(FamilyTypes.Plant))
                {
                    tile.SubScore(FamilyTypes.Plant, 10);
                }
            } },
            { "PLANTRULEDECAY", (o, m) => {
                var tile = m.Tile(o);
                if (tile==null) return;

                tile.Score(FamilyTypes.Fungi, (v) => v/2 )
                    .Score(FamilyTypes.Animal, (v) => v/2 );
            } },
            { "PLANTRULEDEBILITATINGVINES", (o, m) => {
                var tile = m.Tile(o);
                if (tile==null) return;

                tile.ScoringClass.AddToken(ScoringToken.CreateToken(FamilyTypes.Plant, TokenType.AntiDote));
            } },
            { "PLANTRULERAMPANTGROWTH", (o, m) => {
                var tile = m.Tile(o);
                if (tile==null) return;
                var value = tile.FamilyScore(FamilyTypes.Plant)/2;
                if (value==0) value=1;

                foreach(var t in m.Surrounding(o)) t.Score(FamilyTypes.Plant, value);
            } },
            { "PLANTRULEFLOWERINGBLOOM", (o, m) => {
                var tile = m.Tile(o);
                if (tile==null) return;

                foreach(var t in m.Surrounding(o)) {
                    if (t.FamilyRules(FamilyTypes.Plant)) t.AddSettlement(FamilyTypes.Plant, TokenType.Village);
                };
            } },
            { "FUNGIRULECOLONIZATION", (o, m) => {
                var tile = m.Tile(o);
                if (tile==null) return;
                tile.SettlementUpgrade(FamilyTypes.Fungi);

                if( tile.OtherFamilyScore(FamilyTypes.Fungi)>0)
                    tile.Score(FamilyTypes.Fungi, 2);
                else
                    tile.Score(FamilyTypes.Fungi, 10);
            } }
        };

        public GameRulesGGJ2023(ILogger<GameRulesGGJ2023>? logger = null)
        {
            this.logger = logger ?? NullLogger<GameRulesGGJ2023>.Instance;
        }

        public void ExecuteLoop(IGameStatistics gameStatistics)
        {
            // make sure each family has 5 cards
            foreach (FamilyTypes fam in Enum.GetValues(typeof(FamilyTypes)))
            {
                if (!gameStatistics.IsPlayerPlaying(fam)) continue;

                var currentCards = gameStatistics.CurrentInHand(fam);
                while (currentCards.Length < 5)
                {
                    var cardsLeft = gameStatistics.NotPlayedCards(fam);
                    if (cardsLeft.Length == 0)
                    {
                        break;
                    }
                    var cardId = r.Next(cardsLeft.Length);
                    var card = cardsLeft[cardId];
                    gameStatistics.TakeCardInHand(fam, card.Id);
                    currentCards = gameStatistics.CurrentInHand(fam);
                }
                if (currentCards.Length == 0) gameStatistics.PlayerIsPlaying(fam, false);
            }

            if (gameStatistics.NextTurn > DateTime.Now) return;

            gameStatistics.SetNextTime("");
        }
    }
}