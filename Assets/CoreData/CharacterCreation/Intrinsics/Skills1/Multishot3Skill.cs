using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class Multishot3Skill : BaseIntrinsicOption
    {
        private const string _name = ":Multishot3";
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
            public override IRogueMethodRange Range => FacingAnd2FlankingRogueMethodRange.Instance;
            public override int RequiredMP => 3;

            private SortedIntrinsic() : base(0) { }

            public SortedIntrinsic(int lv) : base(lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (CommonAssert.RequireMatchedAmmo(self, arg, AmmoCategories, out var ammo, out _)) return false;
                if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":ActivateSkillMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                }

                self.Main.Stats.Direction = RogueMethodUtility.GetTargetDirection(self, arg);
                if (arg.TryGetTargetPosition(out var targetPosition))
                {
                    this.Throw(ammo, self, activationDepth, targetPosition);
                    this.Throw(ammo, self, activationDepth, targetPosition);
                }
                else
                {
                    this.Throw(ammo, self, activationDepth);
                    this.Throw(ammo, self, activationDepth);
                }
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                additionalEffect = true;
                return 0;
            }
        }
    }
}
