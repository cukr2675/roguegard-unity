using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class MaceAttack : MpSkill
    {
        public override string Name => MainInfoKw.Attack.Name;

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public override int RequiredMp => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;

            // 攻撃力(x2)ダメージの攻撃
            using var damageValue = EffectableValue.Get();
            MainCharacterWorkUtility.TryAddAttack(self);
            StatsEffectedValues.GetAtk(user, damageValue);
            damageValue.MainValue += damageValue.BaseMainValue;
            this.TryHurt(target, user, activationDepth, damageValue);
            this.TryDefeat(target, user, activationDepth, damageValue);
            return true;
        }

        public override int GetAtk(RogueObj self, out bool additionalEffect)
        {
            // 攻撃力+1ダメージの攻撃。
            using var damageValue = EffectableValue.Get();
            StatsEffectedValues.GetAtk(self, damageValue);
            damageValue.MainValue += 1;

            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            additionalEffect = false;
            return hpDamage;
        }
    }
}
