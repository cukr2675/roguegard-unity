using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class PoisonAttack : MPSkill
    {
        [SerializeField] private float _affectRate = 0.1f;

        public override string Name => MainInfoKw.Attack.Name;
        [System.NonSerialized] private string _caption;
        public override string Caption => _caption ??= $"���ʂ̓G�ɍU���̓_���[�W\n{_affectRate * 100f:D}% �œł�t�^";

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public override int RequiredMP => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
            MainCharacterWorkUtility.TryAddAttack(self);

            // �U���̓_���[�W�̍U��
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            this.TryHurt(target, self, activationDepth, damageValue);
            var defeated = this.TryDefeat(target, self, activationDepth, damageValue);

            // �|��Ă��Ȃ���Ίm���œŕt�^
            if (!defeated)
            {
                var randomValue = RogueRandom.Primary.NextFloat(0f, 1f);
                if (randomValue <= _affectRate)
                {
                    this.Affect(target, activationDepth, PoisonStatusEffect.Callback, user: self);
                }
            }

            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            // �U���̓_���[�W�̍U�� + �ŕt�^
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            additionalEffect = true;
            return hpDamage;
        }
    }
}
