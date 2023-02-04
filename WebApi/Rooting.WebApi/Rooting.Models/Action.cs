namespace Rooting.Models
{
    public class ActionBase : RootingModelBase, ICloneable
    {
        public int Cost { get; set; }
        public IEnumerable<Score> Scores { get; set; } = new List<Score>();

        public object Clone()
        {
            var r = new ActionBase
            {
                Cost = Cost,
                Name = Name,
                Uuid = Uuid,
            };
            var scores = new List<Score>();
            foreach (var score in Scores)
            {
                scores.Add((Score)score.Clone());
            }
            r.Scores = scores;
            return r;
        }
    }
}