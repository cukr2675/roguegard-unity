using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class HungerAttack : MPSkill
    {
        [SerializeField] private float _affectRate = 0.1f;
        [SerializeField] private int _nutritionDamage = 50;

        public override string Name => MainInfoKw.Attack.Name;
        [System.NonSerialized] private string _caption;
        public override string Caption => _caption ??= $"���ʂ̓G�ɍU���̓_���[�W\n{_affectRate * 100f:D}% �Ŗ����x�� {_nutritionDamage} �ቺ";

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
            var hit = this.TryHurt(target, self, activationDepth, damageValue);
            var defeated = this.TryDefeat(target, self, activationDepth, damageValue);

            // �|��Ă��Ȃ���Ίm���Ŗ����x�ቺ
            if (hit && !defeated)
            {
                var randomValue = RogueRandom.Primary.NextFloat(0f, 1f);
                if (randomValue <= _affectRate)
                {
                    target.Main.Stats.SetNutrition(target, target.Main.Stats.Nutrition - _nutritionDamage);
                    if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                    {
                        using var handler = h;
                        handler.AppendText(":NutritionDamageMsg::2").AppendText(target).AppendText(_nutritionDamage).AppendText("\n");
                    }
                }
            }

            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            // �U���̓_���[�W�̍U�� + �����x�ቺ
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            additionalEffect = true;
            return hpDamage;
        }
    }
}
