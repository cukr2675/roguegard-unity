using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyStartingItem : IRogueDescription, IMemberable
    {
        IStartingItemOption Option { get; }
        string OptionName { get; }
        Sprite OptionIcon { get; }
        bool OptionColorIsEnabled { get; }
        Color OptionColor { get; }
        string OptionCaption { get; }
        object OptionDetails { get; }
        float GeneratorWeight { get; }
        int Stack { get; }
        IRogueGender OptionGender { get; }
    }
}
