using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="IRogueMethodPassiveAspect"/> に対する user 側の割り込み。
    /// <see cref="IRogueMethodPassiveAspect"/> より先に割り込む。
    /// </summary>
    public interface IRogueMethodActiveAspect
    {
        float Order { get; }

        /// <summary>
        /// このメソッド内で割り込みが発生する可能性がある呼び出しを使用するには、再帰対策が必要。
        /// </summary>
        bool ActiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.ActiveChain chain);
    }
}
