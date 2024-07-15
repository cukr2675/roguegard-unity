using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class SubmachineGunThrow : MPSkill
    {
        public override string Name => MainInfoKw.Throw.Name;

        public override IRogueMethodTarget Target => DependsOnShotRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => DependsOnShotRogueMethodRange.Instance;
        public override Spanning<IKeyword> AmmoCategories => lazyAmmoCategories.Value;
        private static readonly System.Lazy<IKeyword[]> lazyAmmoCategories = new System.Lazy<IKeyword[]>(() => new IKeyword[] { AmmoKw.Bullet });
        public override int RequiredMP => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireMatchedAmmo(self, arg, AmmoCategories, out var ammo, out var ammoInfo)) return false;

            // (基礎攻撃力x2) 回射撃する。
            int count;
            using var atkValue = EffectableValue.Get();
            StatsEffectedValues.GetATK(user, atkValue);
            count = Mathf.FloorToInt(atkValue.BaseMainValue) * 2;
            count = Mathf.Max(count, 1);

            for (int i = 0; i < count; i++)
            {
                if (ammo.Stack == 0) break;

                if (i == 0 && arg.TryGetTargetPosition(out var targetPosition))
                {
                    // 一発目だけ失敗を判定する。
                    var result = this.Shot(ammoInfo, ammo, self, activationDepth, targetPosition, arg.TargetObj);
                    if (!result) return false;
                }
                else
                {
                    this.Shot(ammoInfo, ammo, self, activationDepth);
                }
            }
            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            var ammo = EquipmentUtility.GetAmmo(self, out _);
            throw new System.NotImplementedException();
        }
    }
}
