using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class ThronsTrapSkill : MPSkillIntrinsicOptionScript
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
            public override IRogueMethodRange Range => FacingAnd2FlankingRogueMethodRange.Instance;
            public override int RequiredMP => 3;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (!arg.TryGetTargetPosition(out var targetPosition) ||
                    !RogueDirection.TryFromSign(targetPosition - self.Position, out var targetDirection)) return false;

                self.Main.Stats.Direction = targetDirection;
                var point0 = SpaceUtility.Raycast(
                    self.Location, self.Position, self.Main.Stats.Direction.Rotate(-1), 1, false, false, true, out _, out var pointHit0, out _);
                var point1 = SpaceUtility.Raycast(
                    self.Location, self.Position, self.Main.Stats.Direction, 1, false, false, true, out _, out var pointHit1, out _);
                var point2 = SpaceUtility.Raycast(
                    self.Location, self.Position, self.Main.Stats.Direction.Rotate(+1), 1, false, false, true, out _, out var pointHit2, out _);
                if (point0 && point1 && point2) return false;
                if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":LayTrapMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    MainCharacterWorkUtility.EnqueueViewDequeueState(RogueDevice.Primary.Player);
                }

                var tile = new UserRogueTile(CoreTiles1.ThronsTrap, self);
                if (!point0) { Lay(self, pointHit0, tile, activationDepth); }
                if (!point1) { Lay(self, pointHit1, tile, activationDepth); }
                if (!point2) { Lay(self, pointHit2, tile, activationDepth); }
                return true;
            }

            private void Lay(RogueObj self, Vector2Int pointHit, UserRogueTile trapTile, float activationDepth)
            {
                if (self.Location.Space.TrySet(trapTile, pointHit))
                {
                    // 敷設に成功したとき、その上にいたキャラに状態異常を付与する
                    var obj = self.Location.Space.GetColliderObj(pointHit);
                    if (obj != null) this.StepOn(obj, activationDepth);
                }
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                additionalEffect = true;
                return 0;
            }
        }
    }
}
