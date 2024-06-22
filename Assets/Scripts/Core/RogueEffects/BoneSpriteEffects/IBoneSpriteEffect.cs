using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    /// <summary>
    /// ボーンを変化させるエフェクト。
    /// 新しいボーンを追加するなど、ボーンの構成を変更する場合はこのエフェクトではなく <see cref="IMainInfoSet"/> を使用する。
    /// （同じボーンでも身長などによって異なる位置にしたほうが自然になる可能性があり、その場合は全体の構成を作り直すほうが適切であるため）
    /// </summary>
    public interface IBoneSpriteEffect
    {
        /// <summary>
        /// ボーン自体を変更するときは -100f 前後を指定する。
        /// </summary>
        float Order { get; }

        void AffectSprite(RogueObj self, IReadOnlyNodeBone rootNode, AffectableBoneSpriteTable boneSpriteTable);
    }
}
