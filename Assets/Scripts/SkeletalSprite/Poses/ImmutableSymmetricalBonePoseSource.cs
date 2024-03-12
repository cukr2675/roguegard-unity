using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    /// <summary>
    /// 指定の <see cref="BonePose"/> を４方向に対応させた <see cref="IDirectionalBonePoseSource"/> 。
    /// </summary>
    public class ImmutableSymmetricalBonePoseSource : IDirectionalBonePoseSource
    {
        private readonly BonePose lowerLeftTransformTable;

        private readonly BonePose lowerRightTransformTable;

        private readonly BonePose upperLeftTransformTable;

        private readonly BonePose upperRightTransformTable;

        public ImmutableSymmetricalBonePoseSource(BonePose immutableLowerLeftBonePose, bool nonUp = false)
        {
            lowerLeftTransformTable = immutableLowerLeftBonePose;
            if (!lowerLeftTransformTable.IsImmutable) throw new System.Exception($"{nameof(immutableLowerLeftBonePose)} が Immutable ではありません。");

            var rightDownTransformTable = new BonePose();
            rightDownTransformTable.SetBack(lowerLeftTransformTable.Back);
            var leftUpTransformTable = new BonePose();
            leftUpTransformTable.SetBack(!lowerLeftTransformTable.Back);
            var rightUpTransformTable = new BonePose();
            rightUpTransformTable.SetBack(!lowerLeftTransformTable.Back);
            foreach (var leftDownPair in lowerLeftTransformTable.BoneTransforms)
            {
                var key = leftDownPair.Key;
                var leftDownTransform = leftDownPair.Value;
                if (key == BoneKeyword.Body)
                {
                    // 左右方向で反転するため、 Body のみ反転処理する。
                    var rightDownTransform = CreateMirroredX(leftDownTransform);
                    rightDownTransformTable.AddBoneTransform(rightDownTransform, key);
                    leftUpTransformTable.AddBoneTransform(leftDownTransform, key);
                    rightUpTransformTable.AddBoneTransform(rightDownTransform, key);
                }
                else
                {
                    rightDownTransformTable.AddBoneTransform(leftDownTransform, key);
                    leftUpTransformTable.AddBoneTransform(leftDownTransform, key);
                    rightUpTransformTable.AddBoneTransform(leftDownTransform, key);
                }
            }
            rightDownTransformTable.SetBoneOrder(lowerLeftTransformTable.BoneOrder);
            rightDownTransformTable.SetImmutable();
            leftUpTransformTable.SetBoneOrder(lowerLeftTransformTable.BoneOrder);
            leftUpTransformTable.SetImmutable();
            rightUpTransformTable.SetBoneOrder(lowerLeftTransformTable.BoneOrder);
            rightUpTransformTable.SetImmutable();

            lowerRightTransformTable = rightDownTransformTable;
            if (nonUp)
            {
                upperLeftTransformTable = lowerLeftTransformTable;
                upperRightTransformTable = rightDownTransformTable;
            }
            else
            {
                upperLeftTransformTable = leftUpTransformTable;
                upperRightTransformTable = rightUpTransformTable;
            }

            static BoneTransform CreateMirroredX(BoneTransform t)
            {
                var position = new Vector3(-t.LocalPosition.x, t.LocalPosition.y, t.LocalPosition.z);
                var mirrorX = !t.LocalMirrorX;
                return new BoneTransform(
                    t.Sprite, t.Color, t.OverridesSourceColor, position, t.LocalRotation, t.ScaleOfLocalByLocal,
                    t.TransformsInRootParent, mirrorX, t.LocalMirrorY);
            }
        }

        public BonePose GetBonePose(SpriteDirection direction)
        {
            if (direction == SpriteDirection.Down ||
                direction == SpriteDirection.LowerLeft ||
                direction == SpriteDirection.Left)
            {
                return lowerLeftTransformTable;
            }
            if (direction == SpriteDirection.LowerRight ||
                direction == SpriteDirection.Right)
            {
                return lowerRightTransformTable;
            }
            if (direction == SpriteDirection.UpperLeft)
            {
                return upperLeftTransformTable;
            }
            if (direction == SpriteDirection.Up ||
                direction == SpriteDirection.UpperRight)
            {
                return upperRightTransformTable;
            }
            throw new System.Exception();
        }
    }
}
