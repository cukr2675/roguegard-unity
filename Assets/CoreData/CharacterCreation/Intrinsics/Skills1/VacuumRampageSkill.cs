using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class VacuumRampageSkill : MPSkillIntrinsicOptionScript
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
            public override int RequiredMP => 6;

            private static readonly Vector2Int[] relationalPositions = new[]
            {
                new Vector2Int(-2, -2),
                new Vector2Int(-1, -2),
                new Vector2Int(+0, -2),
                new Vector2Int(+1, -2),
                new Vector2Int(+2, -2),
                new Vector2Int(+2, -1),
                new Vector2Int(+2, +0),
                new Vector2Int(+2, +1),
                new Vector2Int(+2, +2),
                new Vector2Int(+1, +2),
                new Vector2Int(+0, +2),
                new Vector2Int(-1, +2),
                new Vector2Int(-2, +2),
                new Vector2Int(-2, +1),
                new Vector2Int(-2, +0),
                new Vector2Int(-2, -1),
            };

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var visible = MainCharacterWorkUtility.VisibleAt(self.Location, self.Position);
                if (visible)
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":ActivateSkillMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                }



                // 1マス離れた敵を吸い寄せる
                for (int i = 0; i < relationalPositions.Length; i++)
                {
                    var effectPosition = self.Position + relationalPositions[i];
                    var obj = self.Location.Space.GetColliderObj(effectPosition);
                    if (obj == null || !StatsEffectedValues.AreVS(self, obj)) continue;

                    var syncItem = RogueCharacterWork.CreateSyncPositioning(obj);
                    var direction = RogueDirection.FromSignOrLowerLeft(self.Position - obj.Position);
                    if (!SpaceUtility.TryLocate(obj, obj.Location, obj.Position + direction.Forward)) continue;

                    if (visible)
                    {
                        var item = RogueCharacterWork.CreateWalk(obj, obj.Position, obj.Main.Stats.Direction, CoreMotions.FullTurn, true);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, syncItem);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }
                }

                if (visible)
                {
                    MainCharacterWorkUtility.TryAddSkill(self);
                    MainCharacterWorkUtility.TryAddSkill(self);
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
