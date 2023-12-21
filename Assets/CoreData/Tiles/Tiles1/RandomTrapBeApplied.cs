using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RandomTrapBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private RogueTileInfoData[] _traps = null;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (arg.Other is UserRogueTile userTile && !StatsEffectedValues.AreVS(userTile.User, user))
            {
                // 敵対していないキャラが罠を踏んでも起動しないようにする
                return false;
            }
            if (!(arg.Other is IRogueTile tile)) return false;
            if (user.Location.Space.Tilemap.GetTop(user.Position).Info != tile.Info)
            {
                // 無関係のタイルは置き換えない
                return false;
            }

            var trap = RogueRandom.Primary.Choice(_traps);
            var laying = user.Location.Space.TrySet(trap, user.Position, true);
            if (laying && activationDepth < 1f)
            {
                RogueMethodAspectState.Invoke(StdKw.StepOn, trap.Info.BeApplied, null, user, 1f, new(other: trap));
            }
            else if (MainCharacterWorkUtility.VisibleAt(user.Location, user.Position))
            {
                //RogueDevice.Add(DeviceKw.AppendText, ":PaintTrapMsg::2");
                //RogueDevice.Add(DeviceKw.AppendText, user);
                //RogueDevice.Add(DeviceKw.AppendText, CoreTiles1.PaintTrap);
                //RogueDevice.Add(DeviceKw.AppendText, "\n");
            }
            return laying;
        }
    }
}
