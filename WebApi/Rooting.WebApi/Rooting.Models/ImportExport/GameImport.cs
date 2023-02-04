namespace Rooting.Models.ImportExport
{
    public class GameImport
    {
        public IEnumerable<DefineRequirement> Requirements { get; set; } = new List<DefineRequirement>();
        public IEnumerable<DefineAction> Actions { get; set; } = new List<DefineAction>();
        public IEnumerable<DefineCard> Cards { get; set; } = new List<DefineCard>();
    }
}