using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IValueEffect
    {
        float Order { get; }

        /// <summary>
        /// このメソッド内では <see cref="IRogueMethod"/> や値エフェクトなどの再帰する可能性がある呼び出しは禁止。
        /// </summary>
        void AffectValue(IKeyword keyword, EffectableValue value, RogueObj self);
    }
}
