namespace Rooting.Models
{
    public class Score : ICloneable
    {
        public int ScoreValue { get; set; }
        public ScoreType ScoreType { get; set; }
        public FamilyTypes FamilyTypes { get; set; }

        public object Clone()
        {
            return new Score
            {
                ScoreValue = ScoreValue,
                ScoreType = ScoreType,
                FamilyTypes = FamilyTypes
            };
        }
    }
}