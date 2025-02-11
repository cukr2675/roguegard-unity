using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public class DefaultSpritePoseSource : IDirectionalSpritePoseSource
    {
        public static DefaultSpritePoseSource Instance { get; } = new DefaultSpritePoseSource();

        private readonly ImmutableSymmetricalSpritePoseSource source;

        private DefaultSpritePoseSource()
        {
            var pose = new SpritePose();
            var transform = new SpritePoseBoneTransform(null, null, Vector3.zero, Quaternion.identity, Vector3.one, false, false, false);
            pose.AddBoneTransform(transform, BoneKeyword.Body); // Body でキャラ全身を左右反転させるため Body だけは設定する
            pose.SetImmutable();
            source = new ImmutableSymmetricalSpritePoseSource(pose);
        }

        public SpritePose GetSpritePose(SpriteDirection direction)
        {
            return source.GetSpritePose(direction);
        }
    }
}
