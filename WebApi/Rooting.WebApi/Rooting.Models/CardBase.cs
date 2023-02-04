namespace Rooting.Models
{
    public class CardBase : RootingModelBase
    {
        public int TotalCost { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Art { get; set; } = string.Empty;
        public FamilyTypes FamilyTypes { get; set; }
        public ICollection<Action> Actions { get; set; } = Array.Empty<Action>();
        public ICollection<Requirement> Requirements { get; set; } = Array.Empty<Requirement>();
    }
}