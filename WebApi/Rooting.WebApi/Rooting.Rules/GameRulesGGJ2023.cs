using Rooting.Models;
using Microsoft.Extensions.Logging;

namespace Rooting.Rules
{
    public class GameRulesGGJ2023 : IGameEngine
    {
        private readonly ILogger<GameRulesGGJ2023> logger;

        private readonly Random r = new Random();

        public GameRulesGGJ2023(ILogger<GameRulesGGJ2023> logger)
        {
            this.logger = logger;
        }

        public void ExecuteLoop(IGameStatistics gameStatistics)
        {
            if (gameStatistics.Players.Count() < 3)
            {
                gameStatistics.Generation = 0;
                return;
            }

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
                        gameStatistics.PlayerIsPlaying(fam, false);
                        break;
                    }
                    var cardId = r.Next(cardsLeft.Length);
                    var card = cardsLeft[cardId];
                    gameStatistics.TakeCardInHand(fam, card.Id);
                }
            }
        }
    }
}