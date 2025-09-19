using OchalikeSprites;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyExpressiveEyeMember : IReadOnlyMember
    {
        ExpressiveEyeType Type { get; }
    }
}
