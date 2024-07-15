using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard
{
    public class KeywordSpriteMotion : RogueSpriteMotion
    {
        public static KeywordSpriteMotion Wait { get; } = new KeywordSpriteMotion(MainInfoKw.Wait);
        public static KeywordSpriteMotion Walk { get; } = new KeywordSpriteMotion(MainInfoKw.Walk);
        public static KeywordSpriteMotion Attack { get; } = new KeywordSpriteMotion(MainInfoKw.Attack);
        public static KeywordSpriteMotion GunThrow { get; } = new KeywordSpriteMotion(StdKw.GunThrow);
        public static KeywordSpriteMotion Hit { get; } = new KeywordSpriteMotion(MainInfoKw.Hit);
        public static KeywordSpriteMotion Guard { get; } = new KeywordSpriteMotion(StatsKw.Guard);
        public static KeywordSpriteMotion NoDamage { get; } = new KeywordSpriteMotion(StdKw.NoDamage);
        public static KeywordSpriteMotion BeDefeated { get; } = new KeywordSpriteMotion(MainInfoKw.BeDefeated);
        public static KeywordSpriteMotion Victory { get; } = new KeywordSpriteMotion(StdKw.Victory);

        public override IKeyword Keyword { get; }

        public KeywordSpriteMotion(IKeyword keyword)
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
