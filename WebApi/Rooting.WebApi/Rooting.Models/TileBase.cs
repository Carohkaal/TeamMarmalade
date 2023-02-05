using Rooting.Models.ResponseModels;

namespace Rooting.Models
{
    public struct ScoringToken : IScoringToken
    {
        public ScoringToken()
        {
            PlayingState = PlayingState.None;
            Family = FamilyTypes.Any;
            Token = TokenType.None;
        }

        public FamilyTypes Family { get; set; }
        public PlayingState PlayingState { get; set; }
        public TokenType Token { get; set; }

        public bool Play()
        {
            if (PlayingState != PlayingState.InHand) return false;
            PlayingState = PlayingState.Played;
            return true;
        }

        public static IScoringToken CreateToken(FamilyTypes family, TokenType level)
        {
            return new ScoringToken
            {
                PlayingState = PlayingState.Played,
                Family = family,
                Token = level,
            };
        }
    }

    public interface IScoringToken
    {
        FamilyTypes Family { get; }
        TokenType Token { get; }
        PlayingState PlayingState { get; }

        bool Play();
    }

    public class ScoringClass
    {
        public IEnumerable<IScoringToken> Tokens => scoringTokens;
        public Dictionary<FamilyTypes, int> FamilyScore { get; set; } = new();
        public Dictionary<FamilyTypes, int> SubsurfaceScore { get; set; } = new();
        public ICollection<PlayingCard> CardsPlayed => cardsPlayed;

        private readonly List<IScoringToken> scoringTokens = new();
        private readonly List<PlayingCard> cardsPlayed = new List<PlayingCard>();

        public void UpdateTokens(IEnumerable<IScoringToken> tokens)
        {
            scoringTokens.Clear();
            scoringTokens.AddRange(tokens);
        }

        public void AddCard(PlayingCard card)
        {
            card.PlayingState = PlayingState.Played;
            cardsPlayed.Add(card);
        }

        public bool AddToken(IScoringToken token)
        {
            if (token.Play())
            {
                scoringTokens.Add(token);
                return true;
            }
            return false;
        }

        public void SetScore(FamilyTypes type, int score)
        {
            if (FamilyScore.ContainsKey(type))
                FamilyScore[type] = score;
            else
                FamilyScore.Add(type, score);
        }

        public void AddScore(FamilyTypes type, int score)
        {
            if (FamilyScore.ContainsKey(type))
                FamilyScore[type] += score;
            else
                FamilyScore.Add(type, score);
        }

        public void AddSubScore(FamilyTypes type, int score)
        {
            if (SubsurfaceScore.ContainsKey(type))
                SubsurfaceScore[type] += score;
            else
                SubsurfaceScore.Add(type, score);
        }
    }

    public class TileBase : RootingModelBase, ICloneable, IOrigin
    {
        public TileBase()
        {
        }

        public TileBase(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; set; }
        public int Col { get; set; }
        public FamilyTypes FamilyType { get; set; }
        public ScoringClass ScoringClass { get; } = new();

        public object Clone()
        {
            return new TileBase
            {
                FamilyType = FamilyType,
                Col = Col,
                Name = Name,
                Row = Row,
                Uuid = Uuid,
            };
        }
    }
}