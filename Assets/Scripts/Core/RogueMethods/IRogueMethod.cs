using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueMethod
    {
        /// <summary>
        /// 基本的に <see cref="RogueMethodAspectState"/> から実行される。
        /// </summary>
        bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        // 状態を持つことを想定しないため、クローン生成は実装しない。
    }
}
