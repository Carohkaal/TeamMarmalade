namespace Rooting.Models.ResponseModels
{
    public class GameLog
    {
        /// <summary>
        /// The game identification.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Timing en status information for this snapshot.
        /// </summary>
        public DateTime CreatedAtTime { get; set; }
        public DateTime StartedAtTime { get; set; }
        public DateTime CompletedAtTime { get; set; }
        public GameStatus Status { get; set; }
        /// <summary>
        /// The log entries.
        /// </summary>
        public IList<LogEntry> LogEntries { get; set; } = new List<LogEntry>();  
    }
}