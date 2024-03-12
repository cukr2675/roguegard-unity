using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public class DefaultBonePoseSource : IDirectionalBonePoseSource
    {
        public static DefaultBonePoseSource Instance { get; } = new DefaultBonePoseSource();

        private readonly ImmutableSymmetricalBonePoseSource source;

        private DefaultBonePoseSource()
        {
            var table = new BonePose();
            var transform = new BoneTransform(null, default, false, Vector3.zero, Quaternion.identity, Vector3.one, false, false, false);
            table.AddBoneTransform(transform, BoneKeyword.Body); // ÉLÉÉÉâÇÃå¸Ç´Ç≈ç∂âEîΩì]Ç≥ÇπÇÈÇΩÇﬂ Body ÇæÇØÇÕê›íËÇ∑ÇÈ
            table.SetImmutable();
            source = new ImmutableSymmetricalBonePoseSource(table);
        }

        public BonePose GetBonePose(SpriteDirection direction)
        {
            return source.GetBonePose(direction);
        }
    }
}
