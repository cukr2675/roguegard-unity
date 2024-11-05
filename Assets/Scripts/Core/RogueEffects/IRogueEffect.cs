using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="RogueObj"/> にシリアル化しても残るエフェクトを付与する。
    /// 連想配列で保存する <see cref="IRogueObjInfo"/> と違いリストで保存される。
    /// </summary>
    [Objforming.RequireRelationalComponent]
    public interface IRogueEffect
    {
        void Open(RogueObj self);

        /// <summary>
        /// <paramref name="other"/> == null のときも true を返す場合、スタック判定には関わらない。
        /// </summary>
        bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other);

        // ShallowOrDeepCopy だと S から始まってしまう（頻繁に出現する Set... 等と被る）ため DeepOrShallowCopy
        IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf);
        IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj);
    }
}
