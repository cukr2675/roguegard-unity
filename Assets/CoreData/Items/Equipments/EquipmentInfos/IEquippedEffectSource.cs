using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IEquippedEffectSource
    {
        IEquippedEffect CreateOrReuse(RogueObj equipment, IEquippedEffect effect);
    }
}
