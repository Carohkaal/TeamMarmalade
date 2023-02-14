namespace Rooting.Models
{
    public class RootingModelBase
    {
        /// <summary>
        /// External unique identifier for all objects
        /// </summary>
        public Guid Uuid { get; set; } = Guid.Empty;

        /// <summary>
        /// External unique identifier for the game
        /// </summary>
        public Guid GameUuid { get; set; } = Guid.Empty;

        /// <summary>
        /// Name for the item
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Coded (and somewat readible) Identification for the current game
        /// </summary>
        public string GameId { get; set; } = string.Empty;
    }
}