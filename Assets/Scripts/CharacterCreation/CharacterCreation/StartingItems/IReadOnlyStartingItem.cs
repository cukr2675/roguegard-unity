using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyStartingItem : IRogueDescription, IMemberable
    {
        IStartingItemOption Option { get; }
        string OptionName { get; }
        Sprite OptionIcon { get; }
        Color? OptionColor { get; }
        string OptionCaption { get; }
        IRogueDetails OptionDetails { get; }
        float GeneratorWeight { get; }
        int Stack { get; }
        IRogueGender OptionGender { get; }
    }
}
