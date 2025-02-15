using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class ParalyzeAttack : MpSkill
    {
        [SerializeField] private float _affectRate = 0.1f;

        public override string Name => MainInfoKw.Attack.Name;
        [System.NonSerialized] private string _caption;
        public override string Caption => _caption ??= $"正面の敵に攻撃力ダメージ\n{_affectRate * 100f:D}% で麻痺を付与";

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public override int RequiredMp => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
            MainCharacterWorkUtility.TryAddAttack(self);

            // 攻撃力ダメージの攻撃
            using var damageValue = EffectableValue.Get();
            StatsEffectedValues.GetAtk(self, damageValue);
            var hit = this.TryHurt(target, self, activationDepth, damageValue);
            var defeated = this.TryDefeat(target, self, activationDepth, damageValue);

            // 倒れておらず、麻痺にもなっていなければ確率で麻痺付与
            if (hit && !defeated && !target.Main.RogueEffects.TryGetEffect<ParalysisStatusEffect>(out _))
            {
                var randomValue = RogueRandom.Primary.NextFloat(0f, 1f);
                if (randomValue <= _affectRate)
                {
                    this.Affect(target, activationDepth, ParalysisStatusEffect.Callback, user: self);
                }
            }

            return true;
        }

        public override int GetAtk(RogueObj self, out bool additionalEffect)
        {
            // 攻撃力ダメージの攻撃 + 麻痺付与
            using var damageValue = EffectableValue.Get();
            StatsEffectedValues.GetAtk(self, damageValue);
            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            additionalEffect = true;
            return hpDamage;
        }
    }
}
