using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// キャラクターのモーション（<see cref="BonePose"/> を使ったアニメーション）のインターフェース。
    /// 待機アニメーションにも使用するため、状態の種類として扱える値（インスタンス）にする必要がある。
    /// </summary>
    public interface IBoneMotion
    {
        IKeyword Keyword { get; }

        void ApplyTo(IMotionSet motionSet, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion);
    }
}
