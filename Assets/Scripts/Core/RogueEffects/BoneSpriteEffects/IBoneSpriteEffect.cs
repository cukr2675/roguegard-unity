using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public interface IBoneSpriteEffect
    {
        /// <summary>
        /// ボーン自体を変更するときは -100f 前後を指定する。
        /// </summary>
        float Order { get; }

        void AffectSprite(RogueObj self, IReadOnlyNodeBone rootNode, AffectableBoneSpriteTable boneSpriteTable);
    }
}
