namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyRogueTileMember : IReadOnlyMember
    {
        IRogueTile Tile { get; }
    }
}
