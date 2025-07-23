namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyAppearance : IRogueDescribable, IReadOnlyMemberable
    {
        IAppearanceOption Option { get; }
        string CustomName { get; }
        string CustomCaption { get; }
        IRogueDetails CustomDetails { get; }
    }
}
