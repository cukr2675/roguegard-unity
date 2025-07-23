namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyAppearance : IRogueDescription, IReadOnlyMemberable
    {
        IAppearanceOption Option { get; }
        string OptionName { get; }
        string OptionCaption { get; }
        IRogueDetails OptionDetails { get; }
    }
}
