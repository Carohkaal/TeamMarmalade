using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDX.Direct3D9;
using Microsoft.Win32.SafeHandles;
//using System.Windows.Forms;
using System.Threading;

//using SharpDX.Direct3D11;

namespace Rooting.Desktop
{
    public class Game1 : Game
    {
        private enum GameState
        {
            MainMenu,
            Gameplay
        }

        private readonly GameState _state = GameState.Gameplay;

        private const int tileWidth = 512;
        private const int tileHeight = 237;
        private const int cardWidth = 150;
        private const int cardHeight = 200;

        private int mapRows;
        private int mapCols;
        

        private const string gameId = "1";

        private Point gameResolution = new Point(1920, 1080);
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private RenderTarget2D renderTarget;
        private Rectangle renderTargetDestination;

        private SpriteFont _currentFont;
        private readonly RootingWebApiClient _webApiClient;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Uri serverUri = new Uri("https://rootingwebapi.azurewebsites.net");
        private readonly Lazy<CardModel[]> _cardDefinitions;
        private readonly Lazy<PlayerModel[]> _currentPlayers;
        private CardModel[] _cards;
        private PlayerModel[] _players;
        private PlayerModel CurrentPlayer = new();
        private readonly SoundBox SoundBox = new();

        /// <summary>
        ///  My cards
        /// </summary>
        private readonly Dictionary<string, Texture2D> cardTextures;

        private PlayingCard[] cardsInHand = Array.Empty<PlayingCard>();
        private TileBase[] tiles = Array.Empty<TileBase>();

        private Texture2D _startScreen;
        private Texture2D _startButton;
        private Texture2D _mapExample;
        private Texture2D _tileEmpty;

        public static MouseState mouseState;

        private KeyboardState currentKeyboardState;

        private readonly string textBox = ""; //Start with no text

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _httpClient.BaseAddress = serverUri;
            _webApiClient = new RootingWebApiClient(_httpClient);
            _cardDefinitions = new Lazy<CardModel[]>(() =>
            {
                var cards = _webApiClient.CardDefinitionsAsync().GetAwaiter().GetResult();
                return cards.ToArray();
            });

            _currentPlayers = new Lazy<PlayerModel[]>(() =>
            {
                var players = _webApiClient.CurrentPlayersAsync().GetAwaiter().GetResult();
                return players.ToArray();
            });

            cardTextures = new Dictionary<string, Texture2D>();
        }

        private void ResetGame()
        {
            _webApiClient.ResetGameAsync().GetAwaiter().GetResult();
        }

        private void StartGame()
        {
            _webApiClient.StartGameAsync(CurrentPlayer.Uuid.ToString(), "true").GetAwaiter().GetResult();
        }

        private void ClaimPlayer(string myName, string myAvatar, FamilyTypes myFamily)
        {
            CurrentPlayer.Avatar = myAvatar;
            CurrentPlayer.FamilyType = myFamily;
            CurrentPlayer.Name = myName;
            var NewPlayer = _webApiClient.ClaimFamilyAsync(CurrentPlayer).GetAwaiter().GetResult();
            if (NewPlayer.Uuid != Guid.Empty)
            {
                CurrentPlayer = NewPlayer;
            }
        }

        private void LoadCurrentHand()
        {
            var myCards = _webApiClient.CardsToPlayAsync(CurrentPlayer.Uuid.ToString()).GetAwaiter().GetResult();
            cardsInHand = myCards.ToArray();
        }

        private void LoadWorld()
        {
            var World = _webApiClient.WorldAsync(gameId, CurrentPlayer.Uuid.ToString()).GetAwaiter().GetResult();
            mapRows = World.Rows;
            mapCols = World.Cols;
            tiles = World.Tiles.ToArray();
        }

        private CardModel GetCardDefinition(PlayingCard card)
        {
            return _cardDefinitions.Value.Where(m => m.Name == card.Name).FirstOrDefault();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _cards = _cardDefinitions.Value;
            _players = _currentPlayers.Value;

            ResetGame();

            ClaimPlayer("Danny", "", FamilyTypes._1);

            StartGame();

            try
            {
                LoadWorld();
                LoadCurrentHand();
            }
            catch { }
            //Window.TextInput += TextInputHandler;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SoundBox.LoadContent(Content);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _graphics.PreferredBackBufferWidth = gameResolution.X;
            _graphics.PreferredBackBufferHeight = gameResolution.Y;
            _graphics.ApplyChanges();

            renderTarget = new RenderTarget2D(GraphicsDevice, gameResolution.X, gameResolution.Y);
            renderTargetDestination = GetRenderTargetDestination(gameResolution, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            _currentFont = Content.Load<SpriteFont>("Fonts/NeueKabel-Regular12");
            _startScreen = Content.Load<Texture2D>("Startscreen-01");
            _startButton = Content.Load<Texture2D>("Startbutton");
            _tileEmpty = Content.Load<Texture2D>("Tiles/emptytile");
            //_mapExample = Content.Load<Texture2D>("Map_example");
            //_cardTexture = Content.Load<Texture2D>("cards/plants-assimilate");
            foreach (var card in _cardDefinitions.Value)
            {
                try
                {
                    var name = card.Art;
                    var lastIndexOf = name.LastIndexOf('.');

                    //If a cardname contains size info this code can be used
                    //
                    //var firstIndexOf = name.IndexOf('.');
                    //if (firstIndexOf != lastIndexOf)
                    //{
                    //    var fileSizeInfo = name.Substring(firstIndexOf, lastIndexOf);
                    //    cardWidth = Int32.Parse(fileSizeInfo.Substring(0, fileSizeInfo.LastIndexOf('x')));
                    //    cardHeight = Int32.Parse(fileSizeInfo.Substring(fileSizeInfo.LastIndexOf('x'), fileSizeInfo.Length));
                    //}

                    name = name.Substring(0, lastIndexOf);


                    var cardTexture = Content.Load<Texture2D>($"cards/{name}.150x200");
                    if (cardTexture != null)
                        cardTextures.Add(card.Name, cardTexture);
                }
                catch { }
            }

            // TODO: use this.Content to load your game content here
        }

        private void TextInputHandler(object sender, TextInputEventArgs args)
        {
            var pressedKey = args.Key;
            var character = args.Character;

            textBox.Append(character);
        }

        public bool ButtonOnClick(Point mouseClick, bool isClicked, Rectangle r)
        {
            if (r.Contains(mouseClick) && isClicked)
            {
                return true;
            }
            return false;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_state)
            {
                //case GameState.MainMenu:
                //  UpdateMainMenu(gameTime);
                //break;
                case GameState.Gameplay:
                    UpdateGameplay(gameTime);
                    break;
            }
        }

        private void UpdateMainMenu(GameTime deltaTime)
        {
            // Poll for current keyboard state
            currentKeyboardState = Keyboard.GetState();
            // If they hit esc, exit
            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            mouseState = Mouse.GetState();
            var isClicked = mouseState.LeftButton == ButtonState.Pressed;
            ButtonOnClick(mouseState.Position, isClicked, new Rectangle(740, 540, 200, 200));

            // Respond to user input for menu selections, etc
            //if (claimFamily)
            //    _state = GameState.GamePlay;
        }

        private void UpdateGameplay(GameTime deltaTime)
        {
            // Poll for current keyboard state
            currentKeyboardState = Keyboard.GetState();
            // If they hit esc, exit
            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // Respond to user actions in the game.
            // Update enemies
            // Handle collisions
            //if (resetGame)
            //    _state = GameState.MainMenu;
        }

        private Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
        {
            float resolutionRatio = (float)resolution.X / resolution.Y;
            float screenRatio;
            Point bounds = new Point(preferredBackBufferWidth, preferredBackBufferHeight);
            screenRatio = (float)bounds.X / bounds.Y;
            float scale;
            Rectangle rectangle = new Rectangle();

            if (resolutionRatio < screenRatio)
                scale = (float)bounds.Y / resolution.Y;
            else if (resolutionRatio > screenRatio)
                scale = (float)bounds.X / resolution.X;
            else
            {
                // Resolution and window/screen share aspect ratio
                rectangle.Size = bounds;
                return rectangle;
            }
            rectangle.Width = (int)(resolution.X * scale);
            rectangle.Height = (int)(resolution.Y * scale);
            return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
        }

        private static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
        {
            Point delta = outerRectangle.Center - innerRectangle.Center;
            innerRectangle.Offset(delta);
            return innerRectangle;
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (_state)
            {
                //case GameState.MainMenu:
                //  DrawMainMenu(gameTime);
                //break;
                case GameState.Gameplay:
                    DrawGameplay(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }

        private void DrawMainMenu(GameTime deltaTime)
        {
            // Draw the main menu, any active selections, etc
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin();
            _spriteBatch.Draw(renderTarget, renderTargetDestination, Color.White);
            _spriteBatch.Draw(_startScreen, new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_currentFont, "Please input your name and click start", new Vector2(840, 540), Color.Black);
            //_spriteBatch.DrawString(_currentFont, textBox, new Vector2(20, 20), Color.Black);
            _spriteBatch.Draw(_startButton, new Vector2(810, 640), Color.White);
            _spriteBatch.End();
        }

        private void DrawGameplay(GameTime deltaTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin();
            _spriteBatch.Draw(renderTarget, renderTargetDestination, Color.White);

            //_spriteBatch.Draw(_mapExample, new Vector2(0, 0), Color.White);

            //DRAW MAP

            for (int i = 0; i < mapRows; i++)
            {
                for (int j = 0; j < mapCols; j++)
                {
                    _spriteBatch.Draw(_tileEmpty, new Vector2(tileWidth + j * tileWidth, tileHeight + i * tileHeight), Color.White);
                }
            }
              
            //DRAW CARDS 

            for (int i = 0; i < cardsInHand.Length; i++)
            {
                _spriteBatch.Draw(cardTextures[cardsInHand[i].Name], new Vector2(cardWidth + i * cardWidth, 880), Color.White);
            }
            
            _spriteBatch.End();
            }
            _spriteBatch.End();
        }
    }