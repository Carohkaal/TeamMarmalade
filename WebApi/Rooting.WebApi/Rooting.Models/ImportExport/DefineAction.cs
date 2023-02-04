namespace Rooting.Models.ImportExport
{
    public class DefineAction
    {
        public string Name { get; set; } = string.Empty;
        public int Cost { get; set; }
        public DefineFamilyScore[] Scores { get; set; } = Array.Empty<DefineFamilyScore>();
    }
}