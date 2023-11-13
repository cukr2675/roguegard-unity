using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class EmberSkill : BaseIntrinsicOption
    {
        private const string _name = ":Ember";
        protected override float Cost => 1f;
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
            public override IRogueMethodRange Range => FrontCutsCornersRogueMethodRange.Instance;
            public override int RequiredMP => 2;

            private SortedIntrinsic() : base(0) { }

            public SortedIntrinsic(int lv) : base(lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RaycastAssert.RequireTarget(FrontCutsCornersRogueMethodRange.Instance, self, arg, out var target)) return false;
                if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "は");
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "を放った！\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.Pyro);
                }

                // 攻撃力+2ダメージの攻撃
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += 2;
                damageValue.SubValues[MainInfoKw.Skill] = 1f;
                this.TryHurt(target, self, activationDepth, damageValue);
                this.TryDefeat(target, self, activationDepth, damageValue);
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力+2ダメージの攻撃
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += 2;
                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = false;
                return hpDamage;
            }
        }
    }
}
