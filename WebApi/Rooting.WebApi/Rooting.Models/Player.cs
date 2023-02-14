namespace Rooting.Models
{
    public class Player : RootingModelBase, ICloneable
    {
        public FamilyTypes FamilyType { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public Score Score { get; set; } = new Score();
        public int CurrentEnergy { get; set; }
        public int MaxEnergy { get; set; }
        public string RemoteIp { get; set; } = string.Empty;
        public ICollection<PlayingCard> CardsStock { get; set; } = Array.Empty<PlayingCard>();
        public ICollection<PlayingCard> CardsPlayed { get; set; } = Array.Empty<PlayingCard>();
        public bool IsPlaying { get; set; }
        public int EvolvedLevel { get; set; }

        public object Clone()
        {
            var result = new Player
            {
                FamilyType = FamilyType,
                Avatar = Avatar,
                CardsPlayed = new List<PlayingCard>(),
                CardsStock = new List<PlayingCard>(),
                CurrentEnergy = CurrentEnergy,
                EvolvedLevel = EvolvedLevel,
                GameId = GameId,
                IsPlaying = IsPlaying,
                MaxEnergy = MaxEnergy,
                Name = Name,
                RemoteIp = RemoteIp,
                Uuid = Uuid,
                Score = (Score)Score.Clone(),
            };
            foreach (var card in CardsPlayed)
            {
                result.CardsStock.Add((PlayingCard)card.Clone());
            }
            foreach (var card in CardsPlayed)
            {
                result.CardsPlayed.Add((PlayingCard)card.Clone());
            }
            return result;
        }

        public string GameKey() => $"{GameId}:{FamilyType}";
    }
}