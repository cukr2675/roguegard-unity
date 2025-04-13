using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;

namespace Roguegard.Device
{
    public interface ICharacterCreationElementsSubView : IElementsSubView
    {
        ISelectOption LoadPresetOption { get; }
    }
}
