using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class BlitzWaveSkill : MPSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [ObjectFormer.Formable]
        private class SortedIntrinsic : MPSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => LineOfSight10RogueMethodRange.Instance;
            public override int RequiredMP => 3;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                using var predicator = Target.GetPredicator(self, 0f, null);
                RaycastAssert.RequireTarget(predicator, Range, self, arg, out var position);
                if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":ActivateSkillMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                }

                var targets = predicator.GetObjs(position);
                for (int i = 0; i < targets.Count; i++)
                {
                    // 攻撃力(x2)ダメージの攻撃
                    using var damageValue = AffectableValue.Get();
                    StatsEffectedValues.GetATK(self, damageValue);
                    damageValue.MainValue += damageValue.BaseMainValue;
                    damageValue.SubValues[MainInfoKw.Skill] = 1f;
                    var result = this.TryHurt(targets[i], self, activationDepth, damageValue);
                    this.TryDefeat(targets[i], self, activationDepth, damageValue);

                    if (result && !damageValue.SubValues.Is(MainInfoKw.BeDefeated) && RogueRandom.Primary.NextFloat(0f, 1f) <= .3f)
                    {
                        // 倒していなければ 30% でひるみ付与
                        this.Affect(targets[i], activationDepth, ParalysisStatusEffect.Callback, null, self, damageValue);
                    }
                }
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力(x2)ダメージの攻撃
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue;

                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = true;
                return hpDamage;
            }
        }
    }
}
