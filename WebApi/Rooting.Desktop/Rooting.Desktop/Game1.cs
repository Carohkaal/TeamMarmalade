using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private PlayerModel CurrentPLayer = new();

        /// <summary>
        ///  My cards
        /// </summary>
        private Dictionary<string, Texture2D> cardTextures;

        private PlayingCard[] cardsInHand = Array.Empty<PlayingCard>();
        private Texture2D _defaultCard;
        public static GameWindow gw;
        public static MouseState mouseState;
        private bool myBoxHasFocus = true;
        private StringBuilder myTextBoxDisplayCharacters = new StringBuilder();
        //...

        // in load assign the game window to that reference this is so we can have a nice way to have more then one textbox.

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.IsFullScreen = true;
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
        }

        private async Task ClaimPLayer(string myName, string myAvatar, FamilyTypes myFamily)
        {
            CurrentPLayer.Avatar = myAvatar;
            CurrentPLayer.FamilyType = myFamily;
            CurrentPLayer.Name = myName;
            CurrentPLayer = await _webApiClient.ClaimFamilyAsync(CurrentPLayer);
        }

        private async Task LoadCurrentHand()
        {
            var myCards = await _webApiClient.CardsToPlayAsync(CurrentPLayer.Uuid.ToString());
            cardsInHand = myCards.ToArray();
        }

        private CardModel? GetCardDefinition(PlayingCard card)
        {
            return _cardDefinitions.Value.Where(m => m.Name == card.Name).FirstOrDefault();
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
            _currentFont = Content.Load<SpriteFont>("Fonts/NeueKabel-Regular12");
            _defaultCard = Content.Load<Texture2D>("Card1");

            foreach (var c in _cardDefinitions.Value)
            {
                try
                {
                    var cardTexture = Content.Load<Texture2D>(c.Art);
                    if (cardTexture != null)
                        cardTextures.Add(c.Name, cardTexture);
                }
                catch { }
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
            _spriteBatch.DrawString(_currentFont, myBoxHasFocus.ToString(), new Vector2(10, 50), Color.Black);
            _spriteBatch.DrawString(_currentFont, myTextBoxDisplayCharacters, new Vector2(10, 100), Color.Black);
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}