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
        WorldMap WorldMap { get; }
        TimeSpan GameLoopTime { get; }

        void SetNextTime(string shout);

        PlayingCard[] CurrentInHand(FamilyTypes familyType);

        PlayingCard[] NotPlayedCards(FamilyTypes familyType);

        bool IsPlayerPlaying(FamilyTypes familyType);

        void PlayerIsPlaying(FamilyTypes familyType, bool playingStatus);

        void TakeCardInHand(FamilyTypes familyType, int cardId);
    }

    public class GameStatistics : IGameStatistics
    {
        public TimeSpan GameLoopTime { get; private set; }
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
        public WorldMap WorldMap { get; private set; } = new WorldMap();
        public string Shout { get; private set; }

        private readonly Player System = new Player
        {
            Name = "System"
        };

        public GameStatistics(
            IGameDefinitionFactory gameDefinitionFactory,
            IGameEngine gameEngine,
            int loopTimeInSeconds = 15)
        {
            if (loopTimeInSeconds < 1) loopTimeInSeconds = 1;
            if (loopTimeInSeconds > 120) loopTimeInSeconds = 120;
            GameLoopTime = TimeSpan.FromSeconds(loopTimeInSeconds);
            this.gameDefinitionFactory = gameDefinitionFactory;
            this.gameEngine = gameEngine;
            gameSetup = new GameSetup();
            Shout = "Initializing...";
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
                Shout = $"Player {player.Name} claimed {player.FamilyType}";
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
            WorldMap.InitWorld(gameSetup.MapRows(), gameSetup.MapColums(), gameSetup.Tiles());
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

        public GameGeneration ReadGameStatus()
        {
            if (DateTime.Now > NextTurn)
            {
                gameEngine.ExecuteLoop(this);
            }

            return new GameGeneration
            {
                CurrentTime = DateTime.Now,
                GameStatus = this.CurrentGameStatus.ToString(),
                Id = gameId,
                Generation = Generation,
                NextTurn = NextTurn,
                Shout = Shout
            };
        }

        public GameGeneration StartGame(Player player, bool force)
        {
            if (!Players.Any(p => p.Uuid == player.Uuid)) throw new GameException("Invalid player");
            if (Players.Count() == 0) throw new GameException("No players");

            Shout = "Waiting for players";
            var r = new GameGeneration
            {
                GameStatus = CurrentGameStatus.ToString(),
                CurrentTime = DateTime.Now,
                Id = GameId,
                NextTurn = NextTurn,
                Shout = Shout
            };

            if (CurrentGameStatus == GameStatus.WaitingForPlayers)
            {
                if (force)
                {
                    AddGameLog(player, LogLevel.Warning, $"{player.Name} started the game with {Players.Count()} players.");
                    CurrentGameStatus = GameStatus.GameWaitingForEndOfTurn;
                    gameLog.StartedAtTime = DateTime.Now;
                    NextTurn = DateTime.Now.Add(GameLoopTime);
                    Shout = "Game Started";
                    r.GameStatus = GameStatus.GameWaitingForEndOfTurn.ToString();
                    r.NextTurn = NextTurn;
                    r.Shout = Shout;

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
                NextTurn = DateTime.Now.Add(GameLoopTime);
                Shout = "Game Started";
                r.GameStatus = GameStatus.GameWaitingForEndOfTurn.ToString();
                r.NextTurn = NextTurn;
                r.Shout = Shout;

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
                Shout = "Game is paused.";
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = CurrentGameStatus.ToString(),
                    NextTurn = NextTurn,
                    Id = gameId,
                    Shout = Shout
                };
            }

            if (gameStatus == GameStatus.GameWaitingForEndOfTurn && CurrentGameStatus == GameStatus.GamePaused)
            {
                CurrentGameStatus = GameStatus.GameWaitingForEndOfTurn;
                NextTurn = DateTime.Now.Add(GameLoopTime);
                AddGameLog(player, LogLevel.Information, "Game resumed");
                Shout = "Game resuming.";
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = CurrentGameStatus.ToString(),
                    NextTurn = NextTurn,
                    Generation = Generation,
                    Id = gameId,
                    Shout = Shout
                };
            }

            if (gameStatus == GameStatus.GameStopped && CurrentGameStatus != GameStatus.GameCalculation)
            {
                CurrentGameStatus = GameStatus.GameStopped;
                AddGameLog(player, LogLevel.Information, "Game stopped");
                Shout = "Game stopped.";
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = CurrentGameStatus.ToString(),
                    NextTurn = DateTime.MaxValue,
                    Generation = Generation,
                    Id = gameId,
                    Shout = Shout
                };
            }
            else
            {
                AddGameLog(player, LogLevel.Warning, $"Modify state from {CurrentGameStatus} to {gameStatus} failed.");
                Shout = "Game status canot be modified.";
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = CurrentGameStatus.ToString(),
                    NextTurn = NextTurn,
                    Generation = Generation,
                    Id = gameId,
                    Shout = Shout
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

        public WorldMap GetWorldMap(Player player, long gameId) => WorldMap;

        public void SetNextTime(string shout)
        {
            Shout = shout;
            Generation++;
            NextTurn = NextTurn.Add(GameLoopTime);
        }
    }
}