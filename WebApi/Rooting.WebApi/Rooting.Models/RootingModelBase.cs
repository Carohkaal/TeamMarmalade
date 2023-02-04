namespace Rooting.Models
{
    public class RootingModelBase
    {
        /// <summary>
        /// External unique identifier for all objects
        /// </summary>
        public Guid Uuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Name for the item
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}