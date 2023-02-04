namespace Rooting.Models
{
    public class PlayingCard : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public FamilyTypes FamilyType { get; set; }
        public PlayingState PlayingState { get; set; }
        public int PlayedAtTile { get; set; }
        public string Message { get; set; } = string.Empty;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}