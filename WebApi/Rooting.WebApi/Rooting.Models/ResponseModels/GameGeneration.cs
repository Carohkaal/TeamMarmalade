﻿namespace Rooting.Models.ResponseModels
{
    public class GameGeneration
    {
        public long Id { get; set; }
        public string GameStatus { get; set; } = string.Empty;
        public DateTime CurrentTime { get; set; }
        public DateTime NextTurn { get; set; }
        public string Shout { get; set; } = string.Empty;
    }
}