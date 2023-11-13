using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class BaseEquippedEffect : IEquippedEffect
    {
        public virtual void AddEffect(RogueObj equipment, RogueObj owner)
        {
            RogueEffectUtility.AddFromRogueEffect(owner, this);
        }

        public virtual void RemoveEffect(RogueObj equipment, RogueObj owner)
        {
            RogueEffectUtility.Remove(owner, this);
        }
    }
}
