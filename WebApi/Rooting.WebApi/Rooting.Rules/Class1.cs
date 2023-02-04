namespace Rooting.Rules
{
    public class LogLine
    {
        public LogLine()
        {
        }

        public LogLine(LogLineType type, string name, string message)
        {
            Type = type;
            Name = name;
            Message = message;
        }

        public LogLineType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}