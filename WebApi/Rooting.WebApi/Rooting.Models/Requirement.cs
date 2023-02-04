namespace Rooting.Models
{
    public class Requirement : RootingModelBase, ICloneable
    {
        public string Description { get; set; } = string.Empty;
        public int RequireTier { get; set; }
        public bool RequireTileControl { get; set; }
        public int RequireTileDistance { get; set; }
        public FamilyTypes RequireFamily { get; set; }

        public object Clone()
        {
            return new Requirement
            {
                Description = Description,
                RequireTier = RequireTier,
                RequireTileControl = RequireTileControl,
                RequireTileDistance = RequireTileDistance,
                Name = Name,
                RequireFamily = RequireFamily,
                Uuid = Uuid
            };
        }
    }
}