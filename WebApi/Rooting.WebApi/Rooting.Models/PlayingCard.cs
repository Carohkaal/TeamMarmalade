namespace Rooting.Models
{
    public class PlayingCard : ICloneable
    {
        public PlayingCard()
        {
        }

        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public FamilyTypes FamilyType { get; set; } = FamilyTypes.Any;
        public PlayingState PlayingState { get; set; } = PlayingState.None;
        public IOrigin PlayedAtTile { get; set; } = new Origin(-1, -1);
        public string Message { get; set; } = string.Empty;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}