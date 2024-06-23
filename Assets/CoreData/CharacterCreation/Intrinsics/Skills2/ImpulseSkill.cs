using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class ImpulseSkill : MPSkillIntrinsicOptionScript
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
            public override int RequiredMP => 12;

            private const float paralyzeRate = 0.5f;
            private const float maxParalyzeActivationDepth = 10f;

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

                // 周囲8マスに攻撃力(x2)ダメージの雷属性攻撃。
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
                    damageValue.SubValues[ElementKw.Thunder] = 1f;
                    this.TryHurt(obj, self, activationDepth, damageValue);
                    this.TryDefeat(obj, self, activationDepth, damageValue);
                }

                // 周囲8マスに確率で麻痺付与
                // すでに麻痺している場合は無視
                // 付与に成功したときは隣接する敵へ連鎖する
                var random = RogueRandom.Primary;
                Paralyze(this, self, activationDepth, self.Position, random);
                return true;
            }

            private static void Paralyze(ISkill skill, RogueObj self, float activationDepth, Vector2Int center, IRogueRandom random)
            {
                for (int i = 0; i < 8; i++)
                {
                    var direction = new RogueDirection(3 - i);
                    var effectPosition = center + direction.Forward;
                    var obj = self.Location.Space.GetColliderObj(effectPosition);
                    if (obj == null || !StatsEffectedValues.AreVS(self, obj)) continue;

                    var randomValue = random.NextFloat(0f, 1f);
                    if (randomValue > paralyzeRate) continue;

                    skill.TryAffect(obj, activationDepth, ParalysisStatusEffect.Callback, user: self);

                    // 最大 10 連鎖
                    if (activationDepth < maxParalyzeActivationDepth)
                    {
                        Paralyze(skill, self, Mathf.Floor(activationDepth) + 1f, center, random);
                    }
                }
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力(x2)ダメージの攻撃 + 麻痺付与
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue + 2;

                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = true;
                return hpDamage;
            }
        }
	}
}
