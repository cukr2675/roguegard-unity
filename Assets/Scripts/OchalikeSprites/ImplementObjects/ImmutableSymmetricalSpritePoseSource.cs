using UnityEngine;

namespace OchalikeSprites
{
    /// <summary>
    /// 指定の <see cref="SpritePose"/> を４方向に対応させた <see cref="IDirectionalSpritePoseSource"/> 。
    /// </summary>
    public class ImmutableSymmetricalSpritePoseSource : IDirectionalSpritePoseSource
    {
        private readonly SpritePose lowerLeftPose;
        private readonly SpritePose lowerRightPose;
        private readonly SpritePose upperLeftPose;
        private readonly SpritePose upperRightPose;

        public ImmutableSymmetricalSpritePoseSource(SpritePose immutableLowerLeftPose, bool nonUp = false)
        {
            lowerLeftPose = immutableLowerLeftPose;
            if (!lowerLeftPose.IsImmutable) throw new System.InvalidOperationException($"{nameof(immutableLowerLeftPose)} が Immutable ではありません。");

            var rightDownPose = new SpritePose();
            rightDownPose.SetBack(lowerLeftPose.Back);
            var leftUpPose = new SpritePose();
            leftUpPose.SetBack(!lowerLeftPose.Back);
            var rightUpPose = new SpritePose();
            rightUpPose.SetBack(!lowerLeftPose.Back);
            foreach (var leftDownPair in lowerLeftPose.BoneTransforms)
            {
                var key = leftDownPair.Key;
                var leftDownTransform = leftDownPair.Value;
                if (key == BoneKeyword.Body)
                {
                    // 左右方向で反転するため、 Body のみ反転処理する。
                    var rightDownTransform = CreateMirroredX(leftDownTransform);
                    rightDownPose.AddBoneTransform(rightDownTransform, key);
                    leftUpPose.AddBoneTransform(leftDownTransform, key);
                    rightUpPose.AddBoneTransform(rightDownTransform, key);
                }
                else
                {
                    rightDownPose.AddBoneTransform(leftDownTransform, key);
                    leftUpPose.AddBoneTransform(leftDownTransform, key);
                    rightUpPose.AddBoneTransform(leftDownTransform, key);
                }
            }
            rightDownPose.SetBoneOrder(lowerLeftPose.BoneOrder);
            rightDownPose.SetImmutable();
            leftUpPose.SetBoneOrder(lowerLeftPose.BoneOrder);
            leftUpPose.SetImmutable();
            rightUpPose.SetBoneOrder(lowerLeftPose.BoneOrder);
            rightUpPose.SetImmutable();

            lowerRightPose = rightDownPose;
            if (nonUp)
            {
                upperLeftPose = lowerLeftPose;
                upperRightPose = rightDownPose;
            }
            else
            {
                upperLeftPose = leftUpPose;
                upperRightPose = rightUpPose;
            }

            static SpritePoseBoneTransform CreateMirroredX(SpritePoseBoneTransform t)
            {
                var position = new Vector3(-t.LocalPosition.x, t.LocalPosition.y, t.LocalPosition.z);
                var mirrorX = !t.LocalMirrorX;
                return new SpritePoseBoneTransform(
                    t.PoseBareSprite, t.PoseBareColor, position, t.LocalRotation, t.ScaleOfLocalByLocal,
                    t.TransformsInRootParent, mirrorX, t.LocalMirrorY);
            }
        }

        public SpritePose GetSpritePose(SpriteDirection direction)
        {
            if (direction == SpriteDirection.Down ||
                direction == SpriteDirection.LowerLeft ||
                direction == SpriteDirection.Left)
            {
                return lowerLeftPose;
            }
            if (direction == SpriteDirection.LowerRight ||
                direction == SpriteDirection.Right)
            {
                return lowerRightPose;
            }
            if (direction == SpriteDirection.UpperLeft)
            {
                return upperLeftPose;
            }
            if (direction == SpriteDirection.Up ||
                direction == SpriteDirection.UpperRight)
            {
                return upperRightPose;
            }
            throw new System.ArgumentException();
        }
    }
}
