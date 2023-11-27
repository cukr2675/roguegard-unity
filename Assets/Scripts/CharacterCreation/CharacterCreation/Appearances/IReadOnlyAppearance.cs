using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyAppearance : IRogueDescription, IMemberable
    {
        IAppearanceOption Option { get; }
        string OptionName { get; }
        string OptionCaption { get; }
        IRogueDetails OptionDetails { get; }
    }
}
