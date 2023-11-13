using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class CommonAttack : MPSkill
    {
        public override string Name => MainInfoKw.Attack.Name;
        public override string Caption => "正面の敵に攻撃力ダメージ";

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public override int RequiredMP => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
            MainCharacterWorkUtility.TryAddAttack(self);

            // 攻撃力ダメージの攻撃
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            this.TryHurt(target, self, activationDepth, damageValue);
            this.TryDefeat(target, self, activationDepth, damageValue);
            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            // 攻撃力ダメージの攻撃
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            additionalEffect = false;
            return hpDamage;
        }
    }
}
