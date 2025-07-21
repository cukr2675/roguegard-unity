namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyAlphabetTypeMember : IReadOnlyMember
    {
        int TypeIndex { get; }
        string Type { get; }
    }
}
