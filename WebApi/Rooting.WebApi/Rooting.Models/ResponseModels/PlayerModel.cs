namespace Rooting.Models.ResponseModels
{
    public class PlayerModel : RootingModelBase
    {
        public FamilyTypes FamilyType { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}