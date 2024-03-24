using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class RustAttack : MPSkill
    {
        public override string Name => MainInfoKw.Attack.Name;
        public override string Caption => "鉄製の装備を50%で解除させる";

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public override int RequiredMP => 0;

        private static readonly CommonAttack common = new CommonAttack();
        private static readonly List<RogueObj> equipments = new List<RogueObj>();

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
            MainCharacterWorkUtility.TryAddAttack(self);

            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            var hurted = this.TryHurt(target, self, activationDepth, damageValue);
            var defeated = this.TryDefeat(target, self, activationDepth, damageValue);

            // ダメージを与えたとき確率で金属装備解除（倒したときは何もしない）
            if (hurted && !defeated && damageValue.MainValue >= 1f) { Unequip(); }

            return true;

            void Unequip()
            {
                equipments.Clear();
                var equipmentState = target.Main.GetEquipmentState(target);
                for (int i = 0; i < equipmentState.Parts.Count; i++)
                {
                    var part = equipmentState.Parts[i];
                    var length = equipmentState.GetLength(part);
                    for (int j = 0; j < length; j++)
                    {
                        var equipment = equipmentState.GetEquipment(part, j);
                        if (equipment == null) continue;

                        var cost = StatsEffectedValues.GetCost(equipment);
                        if (cost <= 0f) continue;

                        using var materialValue = AffectableValue.Get();
                        StatsEffectedValues.GetMaterial(equipment, materialValue);
                        if (!materialValue.SubValues.Is(MaterialKw.Iron)) continue;

                        equipments.Add(equipment);
                    }
                }
                if (equipments.Count >= 1)
                {
                    var index = RogueRandom.Primary.Next(0, equipments.Count);
                    var equipment = equipments[index];
                    var result = this.TryUnequip(equipment, self, activationDepth);
                    if (!result) return;

                    // 解除に成功したとき装備品を地面に落とす。
                    this.Locate(equipment, self, target.Location, target.Position, activationDepth);
                }
            }
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = true;
            return common.GetATK(self, out _);
        }
    }
}
