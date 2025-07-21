namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyItemMember : IReadOnlyMember
    {
        IReadOnlyStartingItem Item { get; }
    }
}
