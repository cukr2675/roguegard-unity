using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class BeamBulletSkill : BaseIntrinsicOption
    {
        private const string _name = ":BeamBullet";
        protected override float Cost => 2f;
        protected override int Lv => 1;

        public override string Name => _name;

        protected override ISortedIntrinsic CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        [ObjectFormer.Formable]
        private class SortedIntrinsic : SortedIntrinsicMPSkill
        {
            public override string Name => _name;
            public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => LineOfSight10RogueMethodRange.Instance;
            public override int RequiredMP => 2;

            private SortedIntrinsic() : base(0) { }

            public SortedIntrinsic(int lv) : base(lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RaycastAssert.RequireTarget(LineOfSight10RogueMethodRange.Instance, self, arg, out var target)) return false;
                if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":ActivateSkillMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    MainCharacterWorkUtility.TryAddShot(self);
                }

                // 攻撃力(x2)ダメージの攻撃。
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue;
                damageValue.SubValues[MainInfoKw.Skill] = 1f;
                this.TryHurt(target, self, activationDepth, damageValue);
                this.TryDefeat(target, self, activationDepth, damageValue);
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力(x2)ダメージの攻撃。
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue;

                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = false;
                return hpDamage;
            }
        }
    }
}
