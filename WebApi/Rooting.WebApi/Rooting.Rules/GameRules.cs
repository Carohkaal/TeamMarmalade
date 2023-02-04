using Rooting.Models.ResponseModels;
using Rooting.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rooting.Rules
{
    public interface IGameEngine
    {
        void ExecuteLoop(IGameStatistics gameStatistics);
    }

    public class GameRulesGGJ2023 : IGameEngine
    {
        private readonly ILogger<GameRulesGGJ2023> logger;
        private readonly TimeSpan gameLoopTime = TimeSpan.FromSeconds(15);
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

    public interface IGameDefinitionFactory
    {
        GameSetup NewGame(int gameId);
    }

    public interface IGameStatistics
    {
        Guid GameId { get; }
        bool GameStarted { get; }
        int Generation { get; set; }
        IEnumerable<Player> Players { get; }
        DateTime TimeStarted { get; }

        PlayingCard[] CurrentInHand(FamilyTypes familyType);

        PlayingCard[] NotPlayedCards(FamilyTypes familyType);

        bool IsPlayerPlaying(FamilyTypes familyType);

        void PlayerIsPlaying(FamilyTypes familyType, bool playingStatus);

        void TakeCardInHand(FamilyTypes familyType, int cardId);
    }

    public class GameStatistics : IGameStatistics
    {
        private Guid gameId = Guid.NewGuid();
        private readonly ConcurrentDictionary<FamilyTypes, Player> activePlayers = new();
        private readonly IGameDefinitionFactory gameDefinitionFactory;
        private GameSetup gameSetup;
        public Guid GameId => gameId;
        public int Generation { get; set; }
        public DateTime TimeStarted { get; set; }
        public bool GameStarted { get; private set; }
        public IEnumerable<Player> Players => activePlayers.Values;

        public GameStatistics(
            IGameDefinitionFactory gameDefinitionFactory,
            IGameEngine gameEngine)
        {
            this.gameDefinitionFactory = gameDefinitionFactory;
            gameSetup = gameDefinitionFactory.NewGame(1);
        }

        public PlayerModel ClaimPlayer(PlayerModel model, string remoteIp)
        {
            var message = string.Empty;
            var player = new Player
            {
                Uuid = model.Uuid,
                Name = model.Name,
                FamilyType = model.FamilyType,
                Avatar = model.Avatar,
                RemoteIp = remoteIp
            };
            if (activePlayers.Values.Any(m => m.RemoteIp == remoteIp))
            {
                return new PlayerModel
                {
                    Name = player.Name,
                    FamilyType = player.FamilyType,
                    Avatar = player.Avatar,
                    Uuid = Guid.Empty,
                    Message = "This client already claimed a player."
                };
            }
            if (activePlayers.TryAdd(player.FamilyType, player))
            {
                message = $"Claimed {player.FamilyType}";
            }
            else
            {
                player.Uuid = Guid.Empty;
                message = $"Someone already claimed {player.FamilyType}";
            }
            return new PlayerModel
            {
                Name = player.Name,
                FamilyType = player.FamilyType,
                Avatar = player.Avatar,
                Message = message,
                Uuid = player.Uuid
            };
        }

        public void ResetGame()
        {
            gameId = Guid.NewGuid();
            Generation = 0;
            TimeStarted = DateTime.MinValue;
            GameStarted = false;
            activePlayers.Clear();
            gameSetup = gameDefinitionFactory.NewGame(1);
        }

        public Player? Player(Guid playerId)
        {
            return activePlayers.Values.FirstOrDefault(m => m.Uuid == playerId);
        }

        internal void UpdatePlayer(FamilyTypes family, string name, string avatar)
        {
            var player = activePlayers[family];
            player.Name = name;
            player.Avatar = avatar;

            activePlayers.TryUpdate(family, player, player);
        }

        public CardModel[] Cards() => gameSetup.Cards.Values.Select(m => new CardModel
        {
            Art = m.Art,
            Cost = m.TotalCost,
            Description = m.Description,
            Range = m.PlayRange,
            Tier = m.Tier,
            Actions = m.Actions.Select(m => m.Name).ToArray(),
            Requirements = m.Requirements.Select(m => m.Name).ToArray()
        }).ToArray();

        public PlayingCard[] CurrentInHand(FamilyTypes familyType) => gameSetup
            .Deck.Values
            .Where(m => m.FamilyType == familyType && m.PlayingState == PlayingState.InHand)
            .ToArray();

        public PlayingCard[] NotPlayedCards(FamilyTypes familyType) => gameSetup
            .Deck.Values
            .Where(m => m.FamilyType == familyType && m.PlayingState == PlayingState.InStock)
            .ToArray();

        public bool IsPlayerPlaying(FamilyTypes familyType)
        {
            var p = Players.FirstOrDefault(m => m.FamilyType == familyType);
            if (p == null) return false;
            return p.IsPlaying;
        }

        public void PlayerIsPlaying(FamilyTypes familyType, bool playingStatus)
        {
            var p = Players.FirstOrDefault(m => m.FamilyType == familyType);
            if (p == null) return;
            p.IsPlaying = playingStatus;
        }

        public void TakeCardInHand(FamilyTypes familyType, int cardId)
        {
            var card = gameSetup.Deck[cardId];
            if (card.FamilyType != familyType)
            {
                throw new ArgumentException();
            }
            card.PlayingState = PlayingState.InHand;
        }

        internal GameStatus GameStatus()
        {
            throw new NotImplementedException();
        }
    }
}