namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyMember
    {
        IMemberSource Source { get; }

        IMember Clone();
    }
}
