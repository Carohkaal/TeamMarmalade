namespace Rooting.Models
{
    public class FamilyBase : RootingModelBase
    {
        private FamilyTypes FamilyTypes { get; set; }
        public string Art { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}