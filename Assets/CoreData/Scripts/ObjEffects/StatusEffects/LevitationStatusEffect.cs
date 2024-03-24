using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    [Objforming.Formable]
    public class LevitationStatusEffect : TimeLimitedStackableStatusEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new LevitationStatusEffect());

        public override string Name => "浮遊";
        public override IKeyword EffectCategory => null;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 50;

        private static readonly Effect effect = new Effect();

        private LevitationStatusEffect() { }

        public override void Open(RogueObj self)
        {
            base.Open(self);
            RogueEffectUtility.AddFromRogueEffect(self, effect);
        }

        protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            base.RemoveClose(self);
            RogueEffectUtility.Remove(self, effect);
        }

        public static void AddFromInfoSet(RogueObj self)
        {
            RogueEffectUtility.AddFromInfoSet(self, effect);
        }

        public static void MotionApplyTo(
            ISpriteMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref SkeletalSpriteTransform transform)
        {
            Motion.Instance.ApplyTo(motionSet, animationTime, direction, ref transform, out _);
        }

        public static bool Remove(RogueObj self)
        {
            return RogueEffectUtility.Remove(self, effect);
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is LevitationStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new LevitationStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }

        private class Effect : IRogueMethodPassiveAspect, ISpriteMotionEffect
        {
            float IRogueMethodPassiveAspect.Order => 0f;
            float ISpriteMotionEffect.Order => 0f;

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveNext next)
            {
                var tool = arg.Tool;
                if (tool == null)
                {
                    // 道具を使用しない場合関わらない。
                    return next.Invoke(keyword, method, self, user, activationDepth, arg);
                }

                // 道具を所持しているか衝突できる場合、そのまま実行させる。
                // （道具が非所持かつ衝突できない場合、そのメソッドは失敗にする。）
                // 能動行動以外では気にせず実行する。
                // ほかのオブジェクトから呼び出された場合（食べ物を投げ当てられたときなど）も気にせず実行する。
                if (self.Space.Contains(tool) || tool.HasCollider || method is not IActiveRogueMethod || (user != null && self != user))
                {
                    return next.Invoke(keyword, method, self, user, activationDepth, arg);
                }
                else
                {
                    if (RogueDevice.Primary.Player == self)
                    {
                        RogueDevice.Add(DeviceKw.AppendText, "浮いてしまって足元に手が届かない！\n");
                    }
                    return false;
                }
            }

            void ISpriteMotionEffect.ApplyTo(
                ISpriteMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref SkeletalSpriteTransform transform)
            {
                Motion.Instance.ApplyTo(motionSet, animationTime, direction, ref transform, out _);
            }
        }

        private class Motion : RogueSpriteMotion
        {
            public static Motion Instance { get; } = new Motion();

            public override IKeyword Keyword => null;

            private readonly Vector3[] positions = new[]
            {
                new Vector3(0f, 1f) / RoguegardSettings.PixelsPerUnit,
                new Vector3(0f, 2f) / RoguegardSettings.PixelsPerUnit,
                new Vector3(0f, 3f) / RoguegardSettings.PixelsPerUnit,
                new Vector3(0f, 4f) / RoguegardSettings.PixelsPerUnit,
                new Vector3(0f, 3f) / RoguegardSettings.PixelsPerUnit,
                new Vector3(0f, 2f) / RoguegardSettings.PixelsPerUnit,
            };

            public override void ApplyTo(
                ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion)
            {
                var index = animationTime / 8 % positions.Length;
                transform.Position = positions[index] + Vector3.up * (4f / RoguegardSettings.PixelsPerUnit);
                endOfMotion = false;
            }
        }
    }
}
