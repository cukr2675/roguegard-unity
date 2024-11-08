using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public interface ICharacterCreationElementsSubView : IElementsSubView
    {
        ISelectOption LoadPresetOption { get; }
    }
}
