using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class PoisonStatusEffect : TimeLimitedStackableStatusEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new PoisonStatusEffect());

        public override string Name => "毒";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 50;

        private int cooldown;

        private static IBoneMotion _smoke;

        private PoisonStatusEffect() { }

        protected override IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RogueDevice.Primary.VisibleAt(target.Location, target.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "は");
                RogueDevice.Add(DeviceKw.AppendText, this);
                RogueDevice.Add(DeviceKw.AppendText, "を浴びた！\n");
                RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.Poison);
                _smoke ??= new VariantBoneMotion(CoreMotions.Smoke, new Color32(200, 50, 200, 255));
                var work = RogueCharacterWork.CreateEffect(target.Position, _smoke, false);
                RogueDevice.AddWork(DeviceKw.EnqueueWork, work);
            }
            return base.AffectTo(target, user, activationDepth, arg);
        }

        protected override bool Close(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "から");
                RogueDevice.Add(DeviceKw.AppendText, this);
                RogueDevice.Add(DeviceKw.AppendText, "が抜けた\n");
            }
            return base.Close(self);
        }

        protected override RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            cooldown--;
            if (cooldown <= 0)
            {
                // 1ダメージ
                using var damage = AffectableValue.Get();
                damage.Initialize(1f);
                damage.SubValues[StatsKw.GuardRate] = -1000000f; // ガード不可
                this.Hurt(self, null, activationDepth, damage, true);
                this.TryDefeat(self, null, activationDepth, damage);

                // 継続ダメージ
                cooldown = 5;
            }
            return default;
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is PoisonStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new PoisonStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }
    }
}
