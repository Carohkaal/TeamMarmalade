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

    public class ScoreBlock
    {
        public ScoreBlock()
        {
        }

        public int Unspecified { get; set; } = 0;
        public int Distance { get; set; } = 0;
        public int Control { get; set; } = 0;
        public int Capture { get; set; } = 0;
        public int Mission { get; set; } = 0;
        public int SubLevel { get; set; } = 0;

        public int Value => Unspecified + Distance + Control + Capture + Mission + SubLevel;
    }

    public class ScoringClass
    {
        public IEnumerable<IScoringToken> Tokens => scoringTokens;
        public Dictionary<FamilyTypes, ScoreBlock> FamilyScore { get; set; } = new();
        public ICollection<PlayingCard> CardsPlayed => cardsPlayed;

        private readonly List<IScoringToken> scoringTokens = new();
        private readonly List<PlayingCard> cardsPlayed = new List<PlayingCard>();

        public void UpdateTokens(IEnumerable<IScoringToken> tokens)
        {
            scoringTokens.Clear();
            scoringTokens.AddRange(tokens);
        }

        public bool HasToken(TokenType tokenType, FamilyTypes familyTypes)
            => scoringTokens.Any(m => m.Token == tokenType && m.Family == familyTypes);

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

        public void SetScore(FamilyTypes type, ScoreType scoreType, int score)
        {
            if (FamilyScore.ContainsKey(type))
            {
                FamilyScore[type].Unspecified = scoreType == ScoreType.Unspecified ? score : FamilyScore[type].Unspecified;
                FamilyScore[type].Distance = scoreType == ScoreType.Distance ? score : FamilyScore[type].Distance;
                FamilyScore[type].Control = scoreType == ScoreType.Control ? score : FamilyScore[type].Control;
                FamilyScore[type].Capture = scoreType == ScoreType.Capture ? score : FamilyScore[type].Capture;
                FamilyScore[type].Mission = scoreType == ScoreType.Mission ? score : FamilyScore[type].Mission;
                FamilyScore[type].SubLevel = scoreType == ScoreType.SubLevel ? score : FamilyScore[type].SubLevel;
            }
            else
            {
                var newScore = new ScoreBlock
                {
                    Unspecified = scoreType == ScoreType.Unspecified ? score : 0,
                    Distance = scoreType == ScoreType.Distance ? score : 0,
                    Control = scoreType == ScoreType.Control ? score : 0,
                    Capture = scoreType == ScoreType.Capture ? score : 0,
                    Mission = scoreType == ScoreType.Mission ? score : 0,
                    SubLevel = scoreType == ScoreType.SubLevel ? score : 0,
                };
                FamilyScore.Add(type, newScore);
            }
        }

        public void AddScore(FamilyTypes type, ScoreType scoreType, int score)
        {
            if (FamilyScore.ContainsKey(type))
            {
                FamilyScore[type].Unspecified += scoreType == ScoreType.Unspecified ? score : 0;
                FamilyScore[type].Distance += scoreType == ScoreType.Distance ? score : 0;
                FamilyScore[type].Control += scoreType == ScoreType.Control ? score : 0;
                FamilyScore[type].Capture += scoreType == ScoreType.Capture ? score : 0;
                FamilyScore[type].Mission += scoreType == ScoreType.Mission ? score : 0;
                FamilyScore[type].SubLevel += scoreType == ScoreType.SubLevel ? score : 0;
            }
            else
            {
                var newScore = new ScoreBlock
                {
                    Unspecified = scoreType == ScoreType.Unspecified ? score : 0,
                    Distance = scoreType == ScoreType.Distance ? score : 0,
                    Control = scoreType == ScoreType.Control ? score : 0,
                    Capture = scoreType == ScoreType.Capture ? score : 0,
                    Mission = scoreType == ScoreType.Mission ? score : 0,
                    SubLevel = scoreType == ScoreType.SubLevel ? score : 0,
                };
                FamilyScore.Add(type, newScore);
            }
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