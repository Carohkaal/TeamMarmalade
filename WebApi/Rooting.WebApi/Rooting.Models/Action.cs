namespace Rooting.Models
{
    public class ActionBase : RootingModelBase
    {
        public int Cost { get; set; }
        public IEnumerable<Score> Scores { get; set; } = new List<Score>();
    }
}