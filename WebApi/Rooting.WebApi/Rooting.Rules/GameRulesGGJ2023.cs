using Rooting.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rooting.Rules
{
    public class GameRulesGGJ2023 : IGameEngine
    {
        private readonly ILogger<GameRulesGGJ2023> logger;

        private readonly Random r = new Random();

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