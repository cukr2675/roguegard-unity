using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class ConfusionStatusEffect : TimeLimitedStackableStatusEffect, IRogueMethodPassiveAspect, IBoneSpriteEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new ConfusionStatusEffect());

        public override string Name => "混乱";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 10;
        public override ISpriteMotion HeadIcon => CoreMotions.Confusion;

        float IRogueMethodPassiveAspect.Order => -1f; // 引数変更のため -1
        float IBoneSpriteEffect.Order => 0f;

        private static readonly RogueMethodArgumentBuilder argBuilder = new RogueMethodArgumentBuilder();

        private ConfusionStatusEffect() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (MainCharacterWorkUtility.VisibleAt(target.Location, target.Position))
            {
                RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.Confusion);
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "は混乱してしまった！\n");
            }
        }

        protected override void PreAffectedTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            NewAffectTo(target, user, activationDepth, arg, statusEffect);
        }

        bool IRogueMethodPassiveAspect.PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveNext next)
        {
            var angle = RogueRandom.Primary.Next(0, 8);
            self.Main.Stats.Direction = new RogueDirection(angle);
            argBuilder.SetArgument(arg);
            argBuilder.ClearTargetPosition();
            argBuilder.TargetObj = null; // 命中確認を無効化する
            var arg1 = argBuilder.ToArgument();
            return next.Invoke(keyword, method, self, user, activationDepth, arg1);
        }

        void IBoneSpriteEffect.AffectSprite(RogueObj self, IBoneNode boneRoot, AffectableBoneSpriteTable boneSpriteTable)
        {
            CoreBoneSpriteTables.GuruguruEyes.Table.AddTo(boneSpriteTable);
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is ConfusionStatusEffect effect && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new ConfusionStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }
    }
}
