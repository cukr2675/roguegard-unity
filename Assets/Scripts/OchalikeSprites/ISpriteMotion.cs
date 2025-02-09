using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    /// <summary>
    /// キャラクターのモーション（<see cref="SpritePose"/> を使ったアニメーション）のインターフェース。
    /// 待機アニメーションにも使用するため、状態の種類として扱える値（インスタンス）にする必要がある。
    /// </summary>
    public interface ISpriteMotion
    {
        void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion);
    }
}
