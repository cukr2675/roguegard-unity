using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class CrossGuardWandAttack : MPSkill
    {
        public override string Name => MainInfoKw.Attack.Name;

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public override int RequiredMP => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
            MainCharacterWorkUtility.TryAddAttack(self);

            // 攻撃力+1ダメージの攻撃
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            damageValue.MainValue += 1;
            this.TryHurt(target, self, activationDepth, damageValue);

            // MPが半分以上なら攻撃力ダメージの追加攻撃
            if (self.Main.Stats.MP >= StatsEffectedValues.GetMaxMP(self) / 2f && this.TryAny(self))
            {
                var target2 = AttackUtility.GetTargetForward(FrontRogueMethodRange.Instance, self);
                MainCharacterWorkUtility.TryAddAttack(self);
                StatsEffectedValues.GetATK(self, damageValue);
                this.TryHurt(target2, self, activationDepth, damageValue);
                this.TryDefeat(target2, self, activationDepth, damageValue);
            }
            this.TryDefeat(target, self, activationDepth, damageValue);
            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            // 攻撃力+1ダメージの攻撃
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            damageValue.MainValue += 1;

            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            if (self.Main.Stats.MP >= StatsEffectedValues.GetMaxMP(self) / 2f)
            {
                // MPが半分以上なら攻撃力ダメージの追加攻撃
                hpDamage += hpDamage - 1;
            }
            additionalEffect = false;
            return hpDamage;
        }
    }
}
