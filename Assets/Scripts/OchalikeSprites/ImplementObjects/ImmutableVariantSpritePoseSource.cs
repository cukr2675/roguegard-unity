using UnityEngine;

namespace OchalikeSprites
{
    /// <summary>
    /// 指定の <see cref="IDirectionalSpritePoseSource"/> の一部のスプライトと色を変更した <see cref="IDirectionalSpritePoseSource"/> 。
    /// 乗り物にまたがるポーズで乗り物の見た目を反映するときなどに使う。
    /// </summary>
    public class ImmutableVariantSpritePoseSource : IDirectionalSpritePoseSource
    {
        private readonly SpritePose[] poses;

        /// <param name="variantTargetBoneName">変更対象の <see cref="OchalikeBone.Name"/> 。 <see cref="BoneKeyword.Free"/> の場合はすべてのボーンが対象</param>
        public ImmutableVariantSpritePoseSource(
            IDirectionalSpritePoseSource poseSource, BoneKeyword variantTargetBoneName, BoneSprite poseBareSprite, Color? poseBareColor)
        {
            poses = new SpritePose[8];
            for (int i = 0; i < 8; i++)
            {
                poses[i] = GetPose(poseSource, variantTargetBoneName, poseBareSprite, poseBareColor, new SpriteDirection(i));
            }
        }

        private SpritePose GetPose(
            IDirectionalSpritePoseSource poseSource, BoneKeyword variantTargetBoneName, BoneSprite poseBareSprite, Color? poseBareColor, SpriteDirection direction)
        {
            var basePose = poseSource.GetSpritePose(direction);
            if (!basePose.IsImmutable) throw new System.ArgumentException("元のポーズが不変ではありません。", nameof(poseSource));

            var pose = new SpritePose();
            foreach (var pair in basePose.BoneTransforms)
            {
                var boneTransform = pair.Value;
                if (variantTargetBoneName == BoneKeyword.Free || pair.Key == variantTargetBoneName)
                {
                    // 元となる BoneTransform で BareColor を上書きしなければ派生ポーズでも上書きしない
                    // 角度をつけるだけの BoneTransform の色が変わってしまうと使い勝手が悪いため（例: 斬撃エフェクトの色は変えたいが腕の色はそのままにしたい）
                    var overridesOnBoneTransformColor = poseBareColor.HasValue && boneTransform.PoseBareColor.HasValue;

                    boneTransform = new SpritePoseBoneTransform(
                        poseBareSprite ?? boneTransform.PoseBareSprite, overridesOnBoneTransformColor ? poseBareColor : boneTransform.PoseBareColor,
                        boneTransform.LocalPosition, boneTransform.LocalRotation, boneTransform.ScaleOfLocalByLocal,
                        boneTransform.TransformsInRootParent, boneTransform.LocalMirrorX, boneTransform.LocalMirrorY);
                }

                pose.AddBoneTransform(boneTransform, pair.Key);
            }
            pose.SetBack(basePose.Back);
            pose.SetBoneOrder(basePose.BoneOrder);
            pose.SetImmutable();
            return pose;
        }

        public SpritePose GetSpritePose(SpriteDirection direction)
        {
            return poses[(int)direction];
        }
    }
}
