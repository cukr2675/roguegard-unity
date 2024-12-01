using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public class DefaultSpriteMotionPoseSource : IDirectionalSpritePoseSource
    {
        public static DefaultSpriteMotionPoseSource Instance { get; } = new DefaultSpriteMotionPoseSource();

        private readonly ImmutableSymmetricalSpritePoseSource source;

        private DefaultSpriteMotionPoseSource()
        {
            var table = new SpritePose();
            var transform = new BoneTransform(null, default, false, Vector3.zero, Quaternion.identity, Vector3.one, false, false, false);
            table.AddBoneTransform(transform, BoneKeyword.Body); // キャラの向きで左右反転させるため Body だけは設定する
            table.SetImmutable();
            source = new ImmutableSymmetricalSpritePoseSource(table);
        }

        public SpritePose GetSpritePose(SpriteDirection direction)
        {
            return source.GetSpritePose(direction);
        }
    }
}
