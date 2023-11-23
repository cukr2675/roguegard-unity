using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class PoisonSkill : MPSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [ObjectFormer.Formable]
        private class SortedIntrinsic : MPSkillSortedIntrinsic<PoisonSkill>
        {
            public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
            public override int RequiredMP => 2;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
                if (MainCharacterWorkUtility.TryAddAttack(self))
                {
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "は");
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "を放った！\n");
                }

                // 毒付与
                using var value = AffectableValue.Get();
                value.Initialize(0f);
                value.SubValues[MainInfoKw.Skill] = 1f;
                this.TryAffect(target, activationDepth, PoisonStatusEffect.Callback, null, self, value);
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 毒付与
                additionalEffect = true;
                return 0;
            }
        }
    }
}
