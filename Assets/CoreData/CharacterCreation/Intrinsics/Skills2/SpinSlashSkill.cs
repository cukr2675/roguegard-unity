using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class SpinSlashSkill : MPSkillIntrinsicOptionScript
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
            public override IRogueMethodRange Range => Within1TileRogueMethodRange.Instance;
            public override int RequiredMP => 4;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var visible = MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var handler);
                if (visible)
                {
                    handler.AppendText(":ActivateSkillMsg::2").AppendText(self).AppendText(this).AppendText("\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    handler.Dispose();
                }

                // 周囲8マスに攻撃力(x2)+2ダメージの攻撃。
                for (int i = 0; i < 8; i++)
                {
                    var direction = new RogueDirection(3 - i);
                    var effectPosition = self.Position + direction.Forward;
                    var obj = self.Location.Space.GetColliderObj(effectPosition);
                    if (obj == null || !StatsEffectedValues.AreVS(self, obj)) continue;

                    using var damageValue = AffectableValue.Get();
                    StatsEffectedValues.GetATK(self, damageValue);
                    damageValue.MainValue += damageValue.BaseMainValue + 2;
                    damageValue.SubValues[MainInfoKw.Skill] = 1f;
                    this.TryHurt(obj, self, activationDepth, damageValue);
                    this.TryDefeat(obj, self, activationDepth, damageValue);
                }
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力(x2)+2ダメージの攻撃。
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue + 2;

                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = false;
                return hpDamage;
            }
        }
	}
}
