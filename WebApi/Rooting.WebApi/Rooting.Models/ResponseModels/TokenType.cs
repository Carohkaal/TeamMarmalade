namespace Rooting.Models.ResponseModels
{
    public enum TokenType
    {
        None = 0,
        Dwelling = 1,
        Village = 2,
        Town = 3,
        City = 4,
        Capital = 5,
        Metropole = 6,
        AntiDote = 0x10 + 1
    }
}