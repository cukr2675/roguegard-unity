using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class SprintAssaultSkill : MPSkillIntrinsicOptionScript
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
            public override IRogueMethodRange Range => StraightLine10RogueMethodRange.Instance;
            public override int RequiredMP => 3;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                using var predicator = ForEnemyRogueMethodTarget.Instance.GetPredicator(self, 0f, null);
                RaycastAssert.RequireTarget(predicator, StraightLine10RogueMethodRange.Instance, self, arg, out var position);

                var direction = self.Main.Stats.Direction;
                var tileCollide = MovementCalculator.Get(self).HasTileCollider;
                SpaceUtility.Raycast(self.Location, self.Position, direction, 10, true, false, tileCollide, out _, out _, out var targetPosition);
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h1))
                {
                    using var handler = h1;
                    handler.AppendText(":ActivateSkillMsg::2").AppendText(self).AppendText(this).AppendText("\n");
                    handler.EnqueueSE(MainInfoKw.Skill);
                    handler.EnqueueWork(RogueCharacterWork.CreateSyncPositioning(self));
                    handler.EnqueueWork(RogueCharacterWork.CreateWalk(self, targetPosition, direction, KeywordSpriteMotion.Walk, false));
                }

                var targets = predicator.GetObjs(position);
                for (int i = 0; i < targets.Count; i++)
                {
                    // 攻撃力(x2)ダメージの攻撃。
                    using var damageValue = EffectableValue.Get();
                    StatsEffectedValues.GetATK(self, damageValue);
                    damageValue.MainValue += damageValue.BaseMainValue;
                    damageValue.SubValues[MainInfoKw.Skill] = 1f;
                    this.TryHurt(targets[i], self, activationDepth, damageValue);
                    this.TryDefeat(targets[i], self, activationDepth, damageValue);
                }

                if (!SpaceUtility.TryLocate(self, self.Location, targetPosition) &&
                    MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h2))
                {
                    using var handler = h2;
                    handler.EnqueueWork(RogueCharacterWork.CreateWalk(self, self.Position, direction, KeywordSpriteMotion.Walk, false));
                }

                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力(x2)ダメージの攻撃。
                using var damageValue = EffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue;

                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = false;
                return hpDamage;
            }
        }
    }
}
