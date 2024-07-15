using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class KnightJumpSkill : MPSkillIntrinsicOptionScript
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
            public override IRogueMethodRange Range => KnightJumpRogueMethodRange.Instance;
            public override int RequiredMP => 0;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                using var predicator = ForEnemyRogueMethodTarget.Instance.GetPredicator(self, 0f, null);
                RaycastAssert.RequireTarget(predicator, KnightJumpRogueMethodRange.Instance, self, arg, out var position);

                var direction = self.Main.Stats.Direction;
                var targetPosition = self.Position + direction.Forward + direction.Rotate(-1).Forward;
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h1))
                {
                    using var handler = h1;
                    handler.AppendText(":ActivateSkillMsg::2").AppendText(self).AppendText(this).AppendText("\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    MainCharacterWorkUtility.TryAddShot(self);
                    handler.EnqueueWork(RogueCharacterWork.CreateSyncPositioning(self));
                    handler.EnqueueWork(RogueCharacterWork.CreateWalk(self, targetPosition, direction, KeywordSpriteMotion.Walk, false));
                }

                var targets = predicator.GetObjs(position);
                var jumpBack = false;
                if (targets.Count == 0)
                {
                    // 正面2マス先に何もなければそこに移動する
                    jumpBack = !SpaceUtility.TryLocate(self, self.Location, targetPosition);
                }
                else
                {
                    // 正面2マス先に誰かいるときそれに攻撃力(x2)ダメージ
                    using var damage = EffectableValue.Get();
                    StatsEffectedValues.GetATK(self, damage);
                    damage.MainValue += damage.BaseMainValue;
                    default(IAffectRogueMethodCaller).Hurt(targets[0], self, 1f, damage);
                    jumpBack = true;
                }

                if (jumpBack && MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h2))
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
                additionalEffect = true;
                return hpDamage;
            }
        }
    }
}
