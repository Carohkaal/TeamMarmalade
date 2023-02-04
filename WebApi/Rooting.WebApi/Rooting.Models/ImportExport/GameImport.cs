namespace Rooting.Models.ImportExport
{
    public class GameImport
    {
        public IEnumerable<DefineRequirement> Requirements { get; set; } = Array.Empty<DefineRequirement>();
        public IEnumerable<DefineAction> Actions { get; set; } = Array.Empty<DefineAction>();
        public IEnumerable<DefineCard> Cards { get; set; } = Array.Empty<DefineCard>();
        public IEnumerable<DefineDeck> Deck { get; set; } = Array.Empty<DefineDeck>();
    }
}