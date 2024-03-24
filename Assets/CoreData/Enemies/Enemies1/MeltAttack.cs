using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class MeltAttack : MPSkill
    {
        public override string Name => MainInfoKw.Attack.Name;
        public override string Caption => "装備を溶かす";

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public override int RequiredMP => 0;

        private static readonly CommonAttack common = new CommonAttack();

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
            MainCharacterWorkUtility.TryAddAttack(self);

            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            var hurted = this.TryHurt(target, self, activationDepth, damageValue);
            var defeated = this.TryDefeat(target, self, activationDepth, damageValue);

            // 攻撃が命中したとき確率で相手の装備品を溶かす（倒したときは何もしない）
            if (hurted && !defeated)
            {
                var equipment = EquipmentUtility.GetRandomArmor(target, RogueRandom.Primary);
                this.TryAffect(equipment, activationDepth, MeltErosion.Callback, null, self);
            }
            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = true;
            return common.GetATK(self, out _);
        }
    }
}
