namespace Rooting.Models
{
    public class Score : RootingModelBase
    {
        public int Distance { get; set; }
        public int Captures { get; set; }
        public int AreaControl { get; set; }
        public int Missions { get; set; }
    }
}