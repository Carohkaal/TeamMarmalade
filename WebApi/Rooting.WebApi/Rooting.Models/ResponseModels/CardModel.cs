namespace Rooting.Models.ResponseModels
{
    public class CardModel
    {
        public string Name { get; set; } = string.Empty;
        public int Tier { get; set; }
        public int Cost { get; set; }
        public int Range { get; set; }
        public string Art { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string[] Actions { get; set; } = Array.Empty<string>();
        public string[] Requirements { get; set; } = Array.Empty<string>();
    }
}