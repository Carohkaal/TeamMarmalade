using Microsoft.Extensions.Logging;

namespace Rooting.Models.ResponseModels
{
    public class LogEntry
    {
        public LogEntry(Player player, LogLevel severity, string message)
        {
            T = DateTime.Now;
            Player = player.Name;
            Severity = severity.ToString();
            Message = message;
        }

        public LogEntry()
        {
        }

        public DateTime T { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string Player { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}