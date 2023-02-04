namespace Rooting.Models
{
    public class PlayingCard : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public FamilyTypes FamilyType { get; set; }
        public PlayingState PlayingState { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public enum PlayingState
    {
        None = 0,
        InStock = 1,
        InHand = 2,
        Played = 3,
    }
}