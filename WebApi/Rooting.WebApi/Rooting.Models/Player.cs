namespace Rooting.Models
{
    public class Player : RootingModelBase
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
    }
}