using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public interface IStatusEffect : IRogueDescription
    {
        IKeyword EffectCategory { get; }

        /// <summary>
        /// このステータスエフェクトの発生源
        /// </summary>
        RogueObj Effecter { get; }

        IBoneMotion HeadIcon { get; }

        float Order { get; }

        void GetEffectedName(RogueNameBuilder refName, RogueObj self);
    }
}
