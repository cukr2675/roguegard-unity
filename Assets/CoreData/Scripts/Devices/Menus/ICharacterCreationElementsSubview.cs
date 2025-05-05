using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;

namespace Roguegard.Device
{
    public interface ICharacterCreationElementsSubview : IElementsSubview
    {
        ISelectOption LoadPresetOption { get; }
    }
}
