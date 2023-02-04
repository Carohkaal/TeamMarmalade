using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace Rooting.Desktop
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _currentFont;
        private RootingWebApiClient _webApiClient;
        private HttpClient _httpClient = new HttpClient();
        private Uri serverUri = new Uri("https://rootingwebapi.azurewebsites.net");
        private readonly Lazy<CardModel[]> _cardDefinitions;
        private readonly Lazy<PlayerModel[]> _currentPlayers;
        private CardModel[] _cards;
        private PlayerModel[] _players;

        /// <summary>
        ///  My cards
        /// </summary>
        private Dictionary<string, Texture2D> cardTexture;

        private CardModel[] cardsInHand = Array.Empty<CardModel>();

        public static GameWindow gw;
        public static MouseState mouseState;
        private bool myBoxHasFocus = true;
        private StringBuilder myTextBoxDisplayCharacters = new StringBuilder();
        //...

        // in load assign the game window to that reference this is so we can have a nice way to have more then one textbox.

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
            gw = Window;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _currentFont = Content.Load<SpriteFont>("Arial");
            var textures = new Dictionary<string, Texture2D>();
            foreach (var card in cardsInHand)
            {
                try
                {
                    var texture = Content.Load<Texture2D>(card.Art);
                    textures.Add(card.Name, texture);
                }
                catch
                {
                    // no texture
                }
            }

            // TODO: use this.Content to load your game content here
        }

        public static void RegisterFocusedButtonForTextInput(System.EventHandler<TextInputEventArgs> method)
        {
            gw.TextInput += method;
        }

        public static void UnRegisterFocusedButtonForTextInput(System.EventHandler<TextInputEventArgs> method)
        {
            gw.TextInput -= method;
        }

        public void CheckClickOnMyBox(Point mouseClick, bool isClicked, Rectangle r)
        {
            if (r.Contains(mouseClick) && isClicked)
            {
                myBoxHasFocus = !myBoxHasFocus;
                if (myBoxHasFocus)
                    RegisterFocusedButtonForTextInput(OnInput);
                else
                    UnRegisterFocusedButtonForTextInput(OnInput);
            }
        }

        public void OnInput(object sender, TextInputEventArgs e)
        {
            var k = e.Key;
            var c = e.Character;
            myTextBoxDisplayCharacters.Append(c);
            System.Diagnostics.Debug.WriteLine(myTextBoxDisplayCharacters);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mouseState = Mouse.GetState();
            var isClicked = mouseState.LeftButton == ButtonState.Pressed;
            CheckClickOnMyBox(mouseState.Position, isClicked, new Rectangle(0, 0, 200, 200));

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            //_spriteBatch.Draw(cardTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_currentFont, myBoxHasFocus.ToString(), new Vector2(10, 50), Color.Yellow);
            _spriteBatch.DrawString(_currentFont, myTextBoxDisplayCharacters, new Vector2(10, 100), Color.Red);
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}