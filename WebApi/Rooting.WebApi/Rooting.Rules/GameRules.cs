using Rooting.Models.ResponseModels;
using Rooting.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Runtime.Serialization;

namespace Rooting.Rules
{
    public class GameException : ApplicationException
    {
        public GameException()
        { }

        public GameException(string? message) : base(message)
        {
        }

        public GameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected GameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public interface IGameStatistics
    {
        long GameId { get; }
        int Generation { get; set; }
        GameStatus CurrentGameStatus { get; }
        IEnumerable<Player> Players { get; }
        DateTime AutoStartTime { get; }
        DateTime NextTurn { get; }

        PlayingCard[] CurrentInHand(FamilyTypes familyType);

        PlayingCard[] NotPlayedCards(FamilyTypes familyType);

        bool IsPlayerPlaying(FamilyTypes familyType);

        void PlayerIsPlaying(FamilyTypes familyType, bool playingStatus);

        void TakeCardInHand(FamilyTypes familyType, int cardId);
    }

    public class GameStatistics : IGameStatistics
    {
        private readonly TimeSpan gameLoopTime = TimeSpan.FromSeconds(15);
        private long gameId = DateTime.Today.Ticks;
        private readonly ConcurrentDictionary<FamilyTypes, Player> activePlayers = new();
        private readonly IGameDefinitionFactory gameDefinitionFactory;
        private readonly IGameEngine gameEngine;
        private GameSetup gameSetup;
        public long GameId => gameId;
        public int Generation { get; set; }
        public GameStatus CurrentGameStatus { get; private set; }
        public IEnumerable<Player> Players => activePlayers.Values;
        public DateTime AutoStartTime { get; private set; }
        public GameLog gameLog = new();
        public DateTime NextTurn { get; private set; }

        private readonly Player System = new Player
        {
            Name = "System"
        };

        public GameStatistics(
            IGameDefinitionFactory gameDefinitionFactory,
            IGameEngine gameEngine)
        {
            this.gameDefinitionFactory = gameDefinitionFactory;
            this.gameEngine = gameEngine;
            gameSetup = new GameSetup();
            ResetGame();
        }

        public PlayerModel ClaimPlayer(PlayerModel model, string remoteIp)
        {
            if (CurrentGameStatus != GameStatus.WaitingForPlayers)
            {
                return new PlayerModel
                {
                    Name = model.Name,
                    FamilyType = model.FamilyType,
                    Avatar = model.Avatar,
                    Uuid = Guid.Empty,
                    Message = "The server is not accepting new players."
                };
            }

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
            gameId = DateTime.Now.Ticks;
            Generation = 0;
            gameLog.StartedAtTime = DateTime.MinValue;
            CurrentGameStatus = GameStatus.WaitingForPlayers;
            activePlayers.Clear();
            AutoStartTime = DateTime.Now.AddMinutes(20);
            gameLog = new();
            gameSetup = gameDefinitionFactory.NewGame(1);
        }

        public Player? Player(Guid playerId)
        {
            return activePlayers.Values.FirstOrDefault(m => m.Uuid == playerId);
        }

        public void UpdatePlayer(FamilyTypes family, string name, string avatar)
        {
            var player = activePlayers[family];
            player.Name = name;
            player.Avatar = avatar;

            activePlayers.TryUpdate(family, player, player);
        }

        public CardModel[] Cards() => gameSetup.Cards.Values.Select(m => new CardModel
        {
            Name = m.Name,
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

        public GameStatus ReadGameStatus() => CurrentGameStatus;

        public GameGeneration StartGame(Player player, bool force)
        {
            if (!Players.Any(p => p.Uuid == player.Uuid)) throw new GameException("Invalid player");
            if (Players.Count() == 0) throw new GameException("No players");

            var r = new GameGeneration
            {
                GameStatus = CurrentGameStatus,
                CurrentTime = DateTime.Now,
                Id = GameId,
                NextTurn = NextTurn,
            };

            if (CurrentGameStatus == GameStatus.WaitingForPlayers)
            {
                if (force)
                {
                    AddGameLog(player, LogLevel.Warning, $"{player.Name} started the game with {Players.Count()} players.");
                    CurrentGameStatus = GameStatus.GameWaitingForEndOfTurn;
                    gameLog.StartedAtTime = DateTime.Now;
                    NextTurn = DateTime.Now.Add(gameLoopTime);
                    r.GameStatus = GameStatus.GameWaitingForEndOfTurn;
                    r.NextTurn = NextTurn;
                    r.Shout = "Game Started";

                    foreach (var p in Players) player.IsPlaying = true;
                }
                else
                {
                    r.NextTurn = AutoStartTime;
                }
            }
            else if (CurrentGameStatus == GameStatus.GameCanStart)
            {
                AddGameLog(player, LogLevel.Warning, $"{player.Name} started the game.");
                CurrentGameStatus = GameStatus.GameWaitingForEndOfTurn;
                gameLog.StartedAtTime = DateTime.Now;
                NextTurn = DateTime.Now.Add(gameLoopTime);
                r.GameStatus = GameStatus.GameWaitingForEndOfTurn;
                r.NextTurn = NextTurn;
                r.Shout = "Game Started";

                foreach (var p in Players) player.IsPlaying = true;
            }
            else
            {
                AddGameLog(player, LogLevel.Warning, $"{player.Name} tried to restart the game.");
                r.Shout = "Restart not allowed";
            }

            gameEngine.ExecuteLoop(this);
            return r;
        }

        private void AddGameLog(Player player, LogLevel level, string message)
        {
            var entry = new LogEntry(player, level, message);
            gameLog.Status = CurrentGameStatus;
            gameLog.LogEntries.Add(entry);
        }

        public GameGeneration GameStatusUserIntervention(Player player, GameStatus gameStatus)
        {
            if (gameStatus == GameStatus.GamePaused
                && (CurrentGameStatus == GameStatus.GameWaitingForEndOfTurn
                || CurrentGameStatus == GameStatus.GameCalculation
                || CurrentGameStatus == GameStatus.GamePaused))
            {
                CurrentGameStatus = GameStatus.GamePaused;
                AddGameLog(player, LogLevel.Information, "Game paused");
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = CurrentGameStatus,
                    NextTurn = NextTurn,
                    Id = gameId,
                    Shout = "Game is paused."
                };
            }

            if (gameStatus == GameStatus.GameWaitingForEndOfTurn && CurrentGameStatus == GameStatus.GamePaused)
            {
                CurrentGameStatus = GameStatus.GameWaitingForEndOfTurn;
                NextTurn = DateTime.Now.Add(gameLoopTime);
                AddGameLog(player, LogLevel.Information, "Game resumed");
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = CurrentGameStatus,
                    NextTurn = NextTurn,
                    Id = gameId,
                    Shout = "Game resuming."
                };
            }

            if (gameStatus == GameStatus.GameStopped && CurrentGameStatus != GameStatus.GameCalculation)
            {
                CurrentGameStatus = GameStatus.GameStopped;
                AddGameLog(player, LogLevel.Information, "Game stopped");
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = CurrentGameStatus,
                    NextTurn = DateTime.MaxValue,
                    Id = gameId,
                    Shout = "Game terminated."
                };
            }
            else
            {
                AddGameLog(player, LogLevel.Warning, $"Modify state from {CurrentGameStatus} to {gameStatus} failed.");
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = CurrentGameStatus,
                    NextTurn = NextTurn,
                    Id = gameId,
                    Shout = "Game status canot be modified."
                };
            }
        }

        public PlayingCard PlayCard(Player player, PlayingCard playingCard)
        {
            var familyType = player.FamilyType;
            if (playingCard.FamilyType != familyType)
            {
                AddGameLog(System, LogLevel.Error, $"Card {playingCard.Id} is not from {familyType}.");
                playingCard.PlayingState = PlayingState.Error;
                playingCard.Message = "Playing card mismatch with family";
                return playingCard;
            }

            var card = CurrentInHand(familyType).FirstOrDefault(m => m.Id == playingCard.Id);
            if (card != null)
            {
                player.IsPlaying = false;
                card.PlayingState = PlayingState.Played;
                card.PlayedAtTile = playingCard.PlayedAtTile;
                card.Message = playingCard.Message ?? $"Played at {DateTime.Now:HH:mm}";
                return card;
            }
            else
            {
                AddGameLog(System, LogLevel.Warning, $"Could not find card {playingCard.Id} in hand.");
                playingCard.Message = "Not played, Not in hand";
                playingCard.PlayingState = PlayingState.Error;
                return playingCard;
            }
        }

        public GameLog OpenGameLog() => gameLog;
    }
}