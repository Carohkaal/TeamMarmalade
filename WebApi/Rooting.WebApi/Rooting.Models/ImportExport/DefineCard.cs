namespace Rooting.Models.ImportExport
{
    public class DefineCard
    {
        public string Name { get; set; } = string.Empty;
        public int Tier { get; set; }
        public int TotalCost { get; set; }
        public string FamilyType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Art { get; set; } = string.Empty;
        public int PlayRange { get; set; }
        public string[] Actions { get; set; } = Array.Empty<string>();
        public string[] Requirements { get; set; } = Array.Empty<string>();
    }
}