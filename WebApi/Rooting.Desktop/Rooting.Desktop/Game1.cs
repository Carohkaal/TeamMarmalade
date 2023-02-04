using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;

namespace Rooting.Desktop
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private RootingWebApiClient _webApiClient;
        private HttpClient _httpClient = new HttpClient();
        private Uri serverUri = new Uri("https://rootingwebapi.azurewebsites.net");
        private readonly Lazy<CardModel[]> _cardDefinitions;
        private readonly Lazy<PlayerModel[]> _currentPlayers;
        private CardModel[] _cards;
        private PlayerModel[] _players;

        private Texture2D cardTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
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
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _cards = _cardDefinitions.Value;
            _players = _currentPlayers.Value;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            cardTexture = Content.Load<Texture2D>("Card1");


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(cardTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}