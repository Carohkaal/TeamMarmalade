using Rooting.Models;

namespace Rooting.Rules
{
    public class GameSetup
    {
        public void Clear()
        {
            Cards.Clear();
        }

        internal void NotFound(string label, string message)
        {
            logLines.Add(new LogLine(LogLineType.NotFound, label, message));
        }

        internal void Invalid(string label, string message)
        {
            logLines.Add(new LogLine(LogLineType.Invalid, label, message));
        }

        public Dictionary<string, Requirement> Requirements { get; } = new();
        public Dictionary<string, ActionBase> Actions { get; } = new();
        public Dictionary<string, CardBase> Cards { get; } = new();

        public IEnumerable<LogLine> LogLines => logLines;
        private readonly List<LogLine> logLines = new();
    }
}