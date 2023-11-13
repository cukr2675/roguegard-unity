using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public enum StackOption
    {
        Default,

        /// <summary>
        /// <see cref="RogueObj"/> の重さが 1 を超えないようにスタックする
        /// </summary>
        StackUntilMax,

        StackUnlimited,

        NotStack,

        ///// <summary>
        ///// 重さが 1 未満の <see cref="RogueObj"/> は無制限にスタックし、
        ///// それ以外の <see cref="RogueObj"/> はスタックしない
        ///// </summary>
        //StackAmmoUnlimitedOtherUntilMax,

        ///// <summary>
        ///// 重さが 1 未満の <see cref="RogueObj"/> は重さが 1 を超えないようにスタックし、
        ///// それ以外の <see cref="RogueObj"/> は無制限にスタックする
        ///// </summary>
        //StackAmmoUntilMaxOtherUnlimited,
    }
}
