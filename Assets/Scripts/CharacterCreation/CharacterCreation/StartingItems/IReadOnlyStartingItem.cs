using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyStartingItem : IRogueDescribable, IReadOnlyMemberable
    {
        IStartingItemOption Option { get; }
        string CustomName { get; }
        Sprite CustomIcon { get; }
        Color? CustomColor { get; }
        string CustomCaption { get; }
        IRogueDetails CustomDetails { get; }
        float GeneratorWeight { get; }
        int Stack { get; }
        IRogueGender CustomGender { get; }
    }
}
