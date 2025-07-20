using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyIntrinsic : IRogueDescription, IMemberable
    {
        IIntrinsicOption Option { get; }
        string OptionName { get; }
        Sprite OptionIcon { get; }
        Color? OptionColor { get; }
        string OptionCaption { get; }
        IRogueDetails OptionDetails { get; }
    }
}
