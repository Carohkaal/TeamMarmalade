namespace Rooting.Models
{
    public interface IRootingModelBase
    {
        public Guid Uuid { get; }
        string Name { get; }
    }
}