using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    /// <summary>
    /// <see cref="RogueObj"/> の状態だけを見て移動を停止するインターフェース。
    /// </summary>
    public interface IStatedWalkStopper
    {
        bool GetStop(RogueObj self);
    }
}
