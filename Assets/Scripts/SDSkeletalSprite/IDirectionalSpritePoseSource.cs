using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    /// <summary>
    /// ８方向の <see cref="SpritePose"/> を持つインターフェース。ポーズを設定した後でも方向を変更できるようにする。
    /// </summary>
    public interface IDirectionalSpritePoseSource
    {
        SpritePose GetSpritePose(SpriteDirection direction);
    }
}
