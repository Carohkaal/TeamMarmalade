namespace Rooting.Models.ImportExport
{
    public class DefineRequirement
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RequireTier { get; set; }
        public string RequireToken { get; set; } = string.Empty;
        public string RequireFamily { get; set; } = string.Empty;
        public bool RequireTileControl { get; set; }
        public int RequireTileDistance { get; set; }
    }
}