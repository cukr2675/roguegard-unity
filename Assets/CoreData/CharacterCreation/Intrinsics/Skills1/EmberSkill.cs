using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class EmberSkill : MPSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [Objforming.Formable]
        private class SortedIntrinsic : MPSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => FrontCutsCornersRogueMethodRange.Instance;
            public override int RequiredMP => 2;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RaycastAssert.RequireTarget(FrontCutsCornersRogueMethodRange.Instance, self, arg, out var target)) return false;
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    handler.AppendText(self).AppendText("は").AppendText(this).AppendText("を放った！\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    handler.EnqueueSE(StdKw.Pyro);
                }

                // 攻撃力+2ダメージの攻撃
                using var damageValue = EffectableValue.Get();
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
                using var damageValue = EffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += 2;
                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = false;
                return hpDamage;
            }
        }
    }
}
