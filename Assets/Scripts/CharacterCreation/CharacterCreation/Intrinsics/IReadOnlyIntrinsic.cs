using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyIntrinsic : IRogueDescribable, IReadOnlyMemberable
    {
        IIntrinsicOption Option { get; }
        string CustomName { get; }
        Sprite CustomIcon { get; }
        Color? CustomColor { get; }
        string CustomCaption { get; }
        IRogueDetails CustomDetails { get; }
    }
}
