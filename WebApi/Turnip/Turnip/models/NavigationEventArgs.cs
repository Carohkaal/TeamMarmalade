using System;

namespace Turnip.Models
{
    public class PageNavigationEventArgs : EventArgs, IGameEvent
    {
        public PageNavigationEventArgs()
        {
        }

        public PageNavigationEventArgs(string pageName, params string[] arguments)
        {
            Page = pageName;
            Arguments = arguments;
        }

        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string Page { get; set; } = string.Empty;
        public string[] Arguments { get; set; } = Array.Empty<string>();
    }
}