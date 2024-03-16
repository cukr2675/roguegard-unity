using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public class KeywordBoneMotion : RogueSpriteMotion
    {
        public static KeywordBoneMotion Wait { get; } = new KeywordBoneMotion(MainInfoKw.Wait);
        public static KeywordBoneMotion Walk { get; } = new KeywordBoneMotion(MainInfoKw.Walk);
        public static KeywordBoneMotion Attack { get; } = new KeywordBoneMotion(MainInfoKw.Attack);
        public static KeywordBoneMotion GunThrow { get; } = new KeywordBoneMotion(StdKw.GunThrow);
        public static KeywordBoneMotion Hit { get; } = new KeywordBoneMotion(MainInfoKw.Hit);
        public static KeywordBoneMotion Guard { get; } = new KeywordBoneMotion(StatsKw.Guard);
        public static KeywordBoneMotion NoDamage { get; } = new KeywordBoneMotion(StdKw.NoDamage);
        public static KeywordBoneMotion BeDefeated { get; } = new KeywordBoneMotion(MainInfoKw.BeDefeated);
        public static KeywordBoneMotion Victory { get; } = new KeywordBoneMotion(StdKw.Victory);

        public override IKeyword Keyword { get; }

        public KeywordBoneMotion(IKeyword keyword)
        {
            Keyword = keyword;
        }

        public override void ApplyTo(
            ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion)
        {
            motionSet.GetPose(Keyword, animationTime, direction, ref transform, out endOfMotion);
        }
    }
}
