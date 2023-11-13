using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class DaggerAttack : MPSkill
    {
        public override string Name => MainInfoKw.Attack.Name;

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public override int RequiredMP => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;

            // 攻撃力ダメージの2回攻撃
            using var damageValue = AffectableValue.Get();
            MainCharacterWorkUtility.TryAddAttack(self);
            StatsEffectedValues.GetATK(user, damageValue);
            this.TryHurt(target, user, activationDepth, damageValue);

            if (this.TryAny(self))
            {
                var target2 = AttackUtility.GetTargetForward(FrontRogueMethodRange.Instance, self);
                MainCharacterWorkUtility.TryAddAttack(self);
                StatsEffectedValues.GetATK(user, damageValue);
                this.TryHurt(target2, user, activationDepth, damageValue);
                this.TryDefeat(target2, self, activationDepth, damageValue);
            }
            this.TryDefeat(target, user, activationDepth, damageValue);
            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            // 攻撃力+1ダメージの攻撃。
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            damageValue.MainValue += 1;

            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            additionalEffect = false;
            return hpDamage;
        }
    }
}
