namespace Rooting.Models
{
    public class CardBase : RootingModelBase
    {
        public int TotalCost { get; set; }
        public int Tier { get; set; }
        public int PlayRange { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Art { get; set; } = string.Empty;
        public FamilyTypes FamilyTypes { get; set; }
        public ICollection<ActionBase> Actions { get; set; } = Array.Empty<ActionBase>();
        public ICollection<Requirement> Requirements { get; set; } = Array.Empty<Requirement>();
    }

    public enum ScoreType
    {
        None = 0,
        Distance = 1,
        Control = 2,
        Capture = 3,
        Mission = 4,
    }
}