﻿namespace Rooting.Models
{
    [Flags]
    public enum FamilyTypes
    {
        None = 0,
        Plant = 1,
        Fungi = 2,
        Animal = 4,
        All = 255
    }
}