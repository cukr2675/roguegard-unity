using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyIntrinsic : IRogueDescription, IMemberable
    {
        IIntrinsicOption Option { get; }
        string OptionName { get; }
        Sprite OptionIcon { get; }
        bool OptionColorIsEnabled { get; }
        Color OptionColor { get; }
        string OptionCaption { get; }
        object OptionDetails { get; }
    }
}
