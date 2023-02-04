﻿namespace Rooting.Models
{
    public class Requirement : RootingModelBase
    {
        public string Description { get; set; } = string.Empty;
        public int RequireTier { get; set; }
        public bool RequireTileControl { get; set; }
        public int RequireTileDistance { get; set; }
        public FamilyTypes RequireFamily { get; set; }
    }
}