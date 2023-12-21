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
                // �G�΂��Ă��Ȃ��L������㩂𓥂�ł��N�����Ȃ��悤�ɂ���
                return false;
            }
            if (!(arg.Other is IRogueTile tile)) return false;
            if (user.Location.Space.Tilemap.GetTop(user.Position).Info != tile.Info)
            {
                // ���֌W�̃^�C���͒u�������Ȃ�
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
