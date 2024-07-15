using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class BoneSpearAttack : MPSkill
    {
        public override string Name => MainInfoKw.Attack.Name;

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => Away2TilesRogueMethodRange.Instance;
        public override int RequiredMP => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(Away2TilesRogueMethodRange.Instance, self, arg, out var target)) return false;
            MainCharacterWorkUtility.TryAddAttack(self);

            // 攻撃力+1ダメージの攻撃。
            using var damageValue = EffectableValue.Get();
            StatsEffectedValues.GetATK(user, damageValue);
            damageValue.MainValue += 1;
            this.TryHurt(target, user, activationDepth, damageValue);
            this.TryDefeat(target, user, activationDepth, damageValue);
            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            // 攻撃力+1ダメージの攻撃。
            using var damageValue = EffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            damageValue.MainValue += 1;

            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            additionalEffect = false;
            return hpDamage;
        }
    }
}
