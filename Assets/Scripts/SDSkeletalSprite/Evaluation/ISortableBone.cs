using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    /// <summary>
    /// <see cref="BoneSorter"/> のソート対象となるボーンのインターフェース。
    /// </summary>
    public interface ISortableBone<T>
    {
        BoneKeyword Name { get; }

        float NormalOrderInParent { get; }

        float BackOrderInParent { get; }

        ISortableBoneChildren<T> Children { get; }

        /// <summary>
        /// 子も含めて前後反転
        /// </summary>
        BoneBack.Type LocalBack { get; set; }

        int NormalFrontSpriteCount { get; }
        int NormalRearSpriteCount { get; }
        int BackFrontSpriteCount { get; }
        int BackRearSpriteCount { get; }

        int NormalPoseFrontSpriteIndex { get; set; }
        int NormalPoseRearSpriteIndex { get; set; }
        int BackPoseFrontSpriteIndex { get; set; }
        int BackPoseRearSpriteIndex { get; set; }
    }
}
