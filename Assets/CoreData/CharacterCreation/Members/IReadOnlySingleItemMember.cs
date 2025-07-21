namespace Roguegard.CharacterCreation
{
    public interface IReadOnlySingleItemMember : IReadOnlyMember
    {
        IStartingItemOption ItemOption { get; }
    }
}
