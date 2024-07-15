using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public interface ISkeletalSpriteRenderController
    {
        int Count { get; }

        void AdjustBones(int bonesCount);

        void SetBoneSprite(
            int index, string name, Sprite sprite, Color color, bool flipX, bool flipY,
            Vector3 localPosition, Quaternion localRotation, Vector3 localScale);

        void ClearBoneSprites();
    }
}
