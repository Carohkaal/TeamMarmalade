using System;

namespace Turnip.Models
{
    public interface IGameEvent
    {
        DateTime Timestamp { get; }
    }
}