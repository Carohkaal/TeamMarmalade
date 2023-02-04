namespace Rooting.Models
{
    public class Score : ICloneable
    {
        public string ScoreValue { get; set; } = string.Empty;
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