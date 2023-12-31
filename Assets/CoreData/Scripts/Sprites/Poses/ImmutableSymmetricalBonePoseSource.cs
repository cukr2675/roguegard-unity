using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 指定の <see cref="BonePose"/> を４方向に対応させた <see cref="IDirectionalBonePoseSource"/> 。
    /// </summary>
    public class ImmutableSymmetricalBonePoseSource : IDirectionalBonePoseSource
    {
        private BonePose lowerLeftTransformTable;

        private BonePose lowerRightTransformTable;

        private BonePose upperLeftTransformTable;

        private BonePose upperRightTransformTable;

        public ImmutableSymmetricalBonePoseSource(BonePose immutableLowerLeftBonePose, bool nonUp = false)
        {
            lowerLeftTransformTable = immutableLowerLeftBonePose;
            if (!lowerLeftTransformTable.IsImmutable) throw new RogueException($"{nameof(immutableLowerLeftBonePose)} が Immutable ではありません。");

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
                if (key == BoneKw.Body)
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

            this.lowerRightTransformTable = rightDownTransformTable;
            if (nonUp)
            {
                this.upperLeftTransformTable = lowerLeftTransformTable;
                this.upperRightTransformTable = rightDownTransformTable;
            }
            else
            {
                this.upperLeftTransformTable = leftUpTransformTable;
                this.upperRightTransformTable = rightUpTransformTable;
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

        public BonePose GetBonePose(RogueDirection direction)
        {
            if (direction == RogueDirection.Down ||
                direction == RogueDirection.LowerLeft ||
                direction == RogueDirection.Left)
            {
                return lowerLeftTransformTable;
            }
            if (direction == RogueDirection.LowerRight ||
                direction == RogueDirection.Right)
            {
                return lowerRightTransformTable;
            }
            if (direction == RogueDirection.UpperLeft)
            {
                return upperLeftTransformTable;
            }
            if (direction == RogueDirection.Up ||
                direction == RogueDirection.UpperRight)
            {
                return upperRightTransformTable;
            }
            throw new RogueException();
        }
    }
}
