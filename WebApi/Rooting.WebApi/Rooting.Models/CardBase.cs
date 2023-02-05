namespace Rooting.Models
{
    public class CardBase : RootingModelBase, ICloneable
    {
        public int TotalCost { get; set; }
        public int Tier { get; set; }
        public int PlayRange { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Art { get; set; } = string.Empty;
        public int Score { get; set; }
        public FamilyTypes FamilyTypes { get; set; }
        public ICollection<ActionBase> Actions { get; set; } = Array.Empty<ActionBase>();
        public ICollection<Requirement> Requirements { get; set; } = Array.Empty<Requirement>();

        public object Clone()
        {
            var clone = new CardBase
            {
                TotalCost = TotalCost,
                Tier = Tier,
                Score = Score,
                PlayRange = PlayRange,
                Description = Description,
                Art = Art,
                FamilyTypes = FamilyTypes,
                Name = Name,
                Uuid = Uuid
            };
            var a = new List<ActionBase>();
            foreach (var item in Actions)
            {
                a.Add((ActionBase)item.Clone());
            }
            clone.Actions = a;

            var r = new List<Requirement>();
            foreach (var item in Requirements)
            {
                r.Add((Requirement)item.Clone());
            }
            clone.Requirements = r;
            return clone;
        }
    }
}