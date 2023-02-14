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
using Rooting.Rules;

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

    public enum GameUpdateStatus
    {
        None,
        NotFound,
        NotModified,
        Modified
    }

    public class GameState
    {
        public GameState(
            string gameKeyIndex,
            Guid gameId,
            int gameLoopTimeInSeconds,
            int gameCode,
            GameSetup gameSetup,
            IGameEngine gameEngine)
        {
            GameKeyIndex = gameKeyIndex;
            GameId = gameId;
            GameCode = gameCode;
            this.gameEngine = gameEngine;
            this.gameSetup = gameSetup;
            GameLoopTime = TimeSpan.FromSeconds(gameLoopTimeInSeconds);
            AutoStartTime = DateTime.Now.AddMinutes(5);
        }

        private readonly DateTime AutoStartTime;
        private readonly GameSetup gameSetup;
        private readonly IGameEngine gameEngine;

        private readonly Player System = new Player
        {
            Name = "System"
        };

        public WorldMap WorldMap { get; private set; } = new WorldMap();
        public TimeSpan GameLoopTime { get; private set; } = TimeSpan.FromSeconds(60);
        public int Generation { get; private set; }
        public string GameKeyIndex { get; private set; } = string.Empty;
        public Guid GameId { get; private set; } = Guid.Empty;
        public int GameCode { get; }
        public GamePlayState GamePlayState { get; private set; } = GamePlayState.WaitingForPlayers;
        public GameLog gameLog = new();
        public string Shout { get; private set; } = string.Empty;
        public DateTime NextTurn { get; private set; }
        public IEnumerable<Guid> Players { get; } = new List<Guid>();

        public Dictionary<int, PlayingCard> Deck { get; } = new();

        private void AddGameLog(Player player, LogLevel level, string message)
        {
            var entry = new LogEntry(player, level, message);
            gameLog.Status = GamePlayState;
            gameLog.LogEntries.Add(entry);
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
                GameStatus = this.GamePlayState.ToString(),
                Id = GameId,
                Generation = Generation,
                NextTurn = NextTurn,
                Shout = Shout
            };
        }

        public GameGeneration GameStatusUserIntervention(Player player, GamePlayState gameStatus)
        {
            if (gameStatus == GamePlayState.GamePaused
                && (GamePlayState == GamePlayState.GameWaitingForEndOfTurn
                || GamePlayState == GamePlayState.GameCalculation
                || GamePlayState == GamePlayState.GamePaused))
            {
                GamePlayState = GamePlayState.GamePaused;
                AddGameLog(player, LogLevel.Information, "Game paused");
                Shout = "Game is paused.";
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = GamePlayState.ToString(),
                    NextTurn = NextTurn,
                    Id = GameId,
                    Shout = Shout
                };
            }

            if (gameStatus == GamePlayState.GameWaitingForEndOfTurn && GamePlayState == GamePlayState.GamePaused)
            {
                GamePlayState = GamePlayState.GameWaitingForEndOfTurn;
                NextTurn = DateTime.Now.Add(GameLoopTime);
                AddGameLog(player, LogLevel.Information, "Game resumed");
                Shout = "Game resuming.";
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = GamePlayState.ToString(),
                    NextTurn = NextTurn,
                    Generation = Generation,
                    Id = GameId,
                    Shout = Shout
                };
            }

            if (gameStatus == GamePlayState.GameStopped && GamePlayState != GamePlayState.GameCalculation)
            {
                GamePlayState = GamePlayState.GameStopped;
                AddGameLog(player, LogLevel.Information, "Game stopped");
                Shout = "Game stopped.";
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = GamePlayState.ToString(),
                    NextTurn = DateTime.MaxValue,
                    Generation = Generation,
                    Id = GameId,
                    Shout = Shout
                };
            }
            else
            {
                AddGameLog(player, LogLevel.Warning, $"Modify state from {GamePlayState} to {gameStatus} failed.");
                Shout = "Game status canot be modified.";
                return new GameGeneration
                {
                    CurrentTime = DateTime.Now,
                    GameStatus = GamePlayState.ToString(),
                    NextTurn = NextTurn,
                    Generation = Generation,
                    Id = GameId,
                    Shout = Shout
                };
            }
        }

        public GameGeneration StartGame(Player player, bool force)
        {
            if (!Players.Contains(player.Uuid)) throw new GameException("Invalid player");
            if (Players.Count() == 0) throw new GameException("No players");

            Shout = "Waiting for players";
            var r = new GameGeneration
            {
                GameStatus = GamePlayState.ToString(),
                CurrentTime = DateTime.Now,
                Id = GameId,
                NextTurn = NextTurn,
                Shout = Shout
            };

            if (GamePlayState == GamePlayState.WaitingForPlayers)
            {
                if (force)
                {
                    AddGameLog(player, LogLevel.Warning, $"{player.Name} started the game with {Players.Count()} players.");
                    GamePlayState = GamePlayState.GameWaitingForEndOfTurn;
                    gameLog.StartedAtTime = DateTime.Now;
                    NextTurn = DateTime.Now.Add(GameLoopTime);
                    Shout = "Game Started";
                    r.GameStatus = GamePlayState.ToString();
                    r.NextTurn = NextTurn;
                    r.Shout = Shout;

                    foreach (var p in Players) player.IsPlaying = true;
                }
                else
                {
                    r.NextTurn = AutoStartTime;
                }
            }
            else if (GamePlayState == GamePlayState.AllFamiliesRegistered)
            {
                AddGameLog(player, LogLevel.Warning, $"{player.Name} started the game.");
                GamePlayState = GamePlayState.GameWaitingForEndOfTurn;
                gameLog.StartedAtTime = DateTime.Now;
                NextTurn = DateTime.Now.Add(GameLoopTime);
                Shout = "Game Started";
                r.GameStatus = GamePlayState.GameWaitingForEndOfTurn.ToString();
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

        public void TakeCardInHand(Player player, int cardId)
        {
            var card = Deck[cardId];
            if (card.PlayerId != player.Uuid)
            {
                throw new ArgumentException();
            }
            card.PlayingState = PlayingState.InHand;
        }

        public PlayingCard PlayCard(Player player, PlayingCard playingCard, IOrigin origin)
        {
            var tile = WorldMap.Tile(origin);
            if (tile == null || tile.FamilyType == FamilyTypes.Any)
            {
                var result = (PlayingCard)playingCard.Clone();
                result.PlayingState = PlayingState.Error;
                result.Message = $"Invalid tile at {origin.Col}, {origin.Row}";
                return result;
            }

            var familyType = player.FamilyType;
            if (playingCard.FamilyType != familyType)
            {
                AddGameLog(System, LogLevel.Error, $"Card {playingCard.Id} is not from {familyType}.");
                playingCard.PlayingState = PlayingState.Error;
                playingCard.Message = "Playing card mismatch with family";
                return playingCard;
            }

            var card = CurrentInHand(player.Uuid).FirstOrDefault(m => m.Id == playingCard.Id);
            if (card == null)
            {
                var result = (PlayingCard)playingCard.Clone();
                result.PlayingState = PlayingState.Error;
                result.Message = $"Card does not exist: {playingCard.Id}";
                return result;
            }

            if (card.Id <= 0)
            {
                AddGameLog(System, LogLevel.Warning, $"Could not find card {playingCard.Id} in hand.");
                playingCard.Message = "Not played, Not in hand";
                playingCard.PlayingState = PlayingState.Error;
                return playingCard;
            }

            var cardRule = gameSetup.Cards[playingCard.Name.ToUpperInvariant()];
            if (cardRule == null)
            {
                AddGameLog(System, LogLevel.Warning, $"Could not find card rule {playingCard.Name}.");
                playingCard.Message = "Not played, No rule defind";
                playingCard.PlayingState = PlayingState.Error;
                return playingCard;
            }
            (PlayingState state, string? message, int costs) = gameEngine.PlayCard(WorldMap, cardRule, card, tile, player.EvolvedLevel);

            player.IsPlaying = false;
            card.PlayingState = state;
            card.PlayedAtTile = origin;
            card.Message = message ?? $"Played at {DateTime.Now:HH:mm}";
            return card;
        }

        public GameLog OpenGameLog() => gameLog;

        public WorldMap GetWorldMap(Player player, long gameId) => WorldMap;

        public void SetNextTime(string shout)
        {
            Shout = shout;
            Generation++;
            NextTurn = NextTurn.Add(GameLoopTime);
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

        public PlayingCard[] CurrentInHand(Guid playerId) => Deck.Values
            .Where(m => m.PlayerId == playerId && m.PlayingState == PlayingState.InHand)
            .ToArray();

        public PlayingCard[] NotPlayedCards(Guid playerId) => Deck.Values
            .Where(m => m.PlayerId == playerId && m.PlayingState == PlayingState.InStock)
            .ToArray();
    }

    public class GameManagement
    {
        private const int defaultCycleTime = 15;
        private readonly ConcurrentDictionary<string, Guid> activeGameKeys = new();
        private readonly ConcurrentDictionary<Guid, Player> activePlayers = new();

        private readonly ConcurrentDictionary<string, Guid> activeGamesIdentifiers = new();
        private readonly ConcurrentDictionary<Guid, GameState> activeGameState = new();

        private readonly IGameDefinitionFactory gameDefinitionFactory;
        private readonly IGameEngine gameEngine;

        public IEnumerable<Player> Players => activePlayers.Values;

        public GameManagement(
            IGameDefinitionFactory gameDefinitionFactory,
            IGameEngine gameEngine)
        {
            this.gameDefinitionFactory = gameDefinitionFactory;
            this.gameEngine = gameEngine;
        }

        public PlayerModel ClaimPlayer(PlayerModel model, string remoteIp)
        {
            // Define player model
            var player = new Player
            {
                Uuid = Guid.NewGuid(),
                GameUuid = Guid.Empty,
                Name = model.Name,
                FamilyType = model.FamilyType,
                Avatar = model.Avatar,
                RemoteIp = remoteIp,
            };

            // Validate player not playing
            if (activePlayers.Values.Any(m => m.RemoteIp == remoteIp))
            {
                return new PlayerModel
                {
                    Name = player.Name,
                    FamilyType = player.FamilyType,
                    Avatar = player.Avatar,
                    Uuid = Guid.Empty,
                    GameUuid = Guid.Empty,
                    Message = "This client already claimed a player."
                };
            }

            // Claim game Uuid
            var gameUuid = activeGamesIdentifiers.GetOrAdd(player.GameId, Guid.NewGuid());
            var gameState = activeGameState.GetOrAdd(gameUuid, new GameState(player.GameId, gameUuid, defaultCycleTime, 1, gameDefinitionFactory.NewGame(1), gameEngine));
            if (gameState.GamePlayState != GamePlayState.WaitingForPlayers)
            {
                return new PlayerModel
                {
                    Name = player.Name,
                    FamilyType = player.FamilyType,
                    Avatar = player.Avatar,
                    Uuid = Guid.Empty,
                    GameUuid = gameUuid,
                    Message = "The server is not accepting new players."
                };
            }

            // Find if familty claimed for this game
            var gameKeyIndex = player.GameKey();
            if (activeGameKeys.TryGetValue(gameKeyIndex, out _))
            {
                return new PlayerModel
                {
                    Name = player.Name,
                    FamilyType = player.FamilyType,
                    Avatar = player.Avatar,
                    Uuid = Guid.Empty,
                    GameUuid = Guid.Empty,
                    Message = $"Someone already claimed {player.FamilyType} for this game"
                };
            };

            // Register familty on game
            if (!activeGameKeys.TryAdd(gameKeyIndex, player.Uuid))
            {
                return new PlayerModel
                {
                    Name = player.Name,
                    FamilyType = player.FamilyType,
                    Avatar = player.Avatar,
                    Uuid = Guid.Empty,
                    GameUuid = Guid.Empty,
                    Message = $"Someone already claimed {player.FamilyType}"
                };
            }

            // Player claimed
            return new PlayerModel
            {
                Name = player.Name,
                FamilyType = player.FamilyType,
                Avatar = player.Avatar,
                Uuid = player.Uuid,
                GameUuid = gameUuid
            };
        }

        public GameUpdateStatus ResetGame(Guid gameId)
        {
            if (gameId == Guid.Empty) return GameUpdateStatus.NotFound;

            var game = activeGameState[gameId];
            if (game == null) return GameUpdateStatus.NotFound;

            var newGame = new GameState(
                game.GameKeyIndex,
                game.GameId,
                (int)game.GameLoopTime.TotalSeconds,
                game.GameCode,
                gameDefinitionFactory.NewGame(game.GameCode),
                gameEngine
            );
            if (!activeGameState.TryUpdate(gameId, game, newGame)) return GameUpdateStatus.NotModified;
            return GameUpdateStatus.Modified;
        }

        public Player? Player(Guid playerId)
        {
            return (activePlayers.TryGetValue(playerId, out var value))
                ? value
                : null;
        }

        public void UpdatePlayer(Guid playerId, string name, string avatar)
        {
            var player = Player(playerId); if (player == null) return;
            player.Name = name;
            player.Avatar = avatar;

            activePlayers.TryUpdate(playerId, player, player);
        }

        public PlayerModel? ResignPlayer(Guid playerId)
        {
            var player = Player(playerId);
            if (player == null) return null;
            var newPlayer = (Player)player.Clone();
            newPlayer.IsPlaying = false;
            var playerInfo = activePlayers.TryUpdate(playerId, newPlayer, player)
                ? newPlayer
                : player;
            return new PlayerModel
            {
                Avatar = playerInfo.Avatar,
                FamilyType = playerInfo.FamilyType,
                GameId = playerInfo.GameId,
                Message = playerInfo.IsPlaying ? "Still playing" : "Resigned",
                Name = playerInfo.Name,
                Uuid = playerInfo.Uuid
            };
        }

        public bool IsPlayerPlaying(Guid playerId)
        {
            var player = Player(playerId);
            if (player == null) return false;

            return player.IsPlaying;
        }

        public void PlayerIsPlaying(Guid playerId, bool playingStatus)
        {
            var player = Player(playerId);
            if (player == null) return;

            var newStatus = (Player)player.Clone();
            player.IsPlaying = playingStatus;
            activePlayers.TryUpdate(playerId, newStatus, player);
        }
    }
}