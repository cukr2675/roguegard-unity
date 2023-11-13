using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class PaintTrapSkill : BaseIntrinsicOption
    {
        private const string _name = ":PaintTrap";
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
            public override IRogueMethodTarget Target => null;
            public override IRogueMethodRange Range => null;
            public override int RequiredMP => 1;

            private SortedIntrinsic() : base(0) { }

            public SortedIntrinsic(int lv) : base(lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (self.Location == null || self.Location.Space.TileCollideAt(self.Position, RogueTileLayer.Building, false)) return false;
                if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":LayTrapMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    MainCharacterWorkUtility.EnqueueViewDequeueState(RogueDevice.Primary.Player);
                }

                var tile = new UserRogueTile(CoreTiles1.PaintTrap, self);
                self.Location.Space.TrySet(tile, self.Position);
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
