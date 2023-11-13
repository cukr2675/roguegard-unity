using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// ８方向の <see cref="BonePose"/> を持つインターフェース。ポーズを設定した後でも方向を変更できるようにする。
    /// </summary>
    public interface IDirectionalBonePoseSource
    {
        BonePose GetBonePose(RogueDirection direction);
    }
}
