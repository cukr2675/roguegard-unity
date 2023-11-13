using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueMethodPassiveAspect
    {
        float Order { get; }

        /// <summary>
        /// このメソッド内で割り込みが発生する可能性がある呼び出しを使用するには、再帰対策が必要。
        /// </summary>
        bool PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveNext next);
    }

    // Walk など実行回数が多いキーワードだけ別リストにするなら必要。
    //interface IReservedRogueMethodPassiveAspect : IRogueMethodPassiveAspect
    //{
    //    /// <summary>
    //    /// このアスペクトが指定の <see cref="IKeyword"/> に対し割り込みを実行する可能性があるかを取得する。
    //    /// </summary>
    //    bool GetReserves(IKeyword keyword);
    //}
}
