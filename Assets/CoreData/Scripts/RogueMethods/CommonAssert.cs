using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommonAssert
    {
        public static bool ObjDoesNotHaveToolAbility(RogueObj obj)
        {
            var equipmentState = obj.Main.GetEquipmentState(obj);
            if (equipmentState == null || equipmentState.GetLength(EquipKw.Weapon) == -1)
            {
                if (RogueDevice.Primary.Player == obj)
                {
                    RogueDevice.Add(DeviceKw.AppendText, obj);
                    RogueDevice.Add(DeviceKw.AppendText, "は道具を使おうとしたがうまくいかない！");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool RequireTool(in RogueMethodArgument arg, out RogueObj tool)
        {
            tool = arg.Tool;
            if (tool == null)
            {
                Debug.Log("使用する道具が見つかりません。");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool RequireTool(IKeyword category, in RogueMethodArgument arg, out RogueObj tool, out IApplyRogueMethod beAppliedMethod)
        {
            tool = arg.Tool;
            beAppliedMethod = default;
            if (tool == null)
            {
                Debug.Log("使用する道具が見つかりません。");
                return true;
            }
            else if (arg.Tool.Main.Category != category)
            {
                Debug.LogError($"{arg.Tool} のカテゴリは {category} ではありません。");
                return true;
            }
            else
            {
                beAppliedMethod = tool.Main.InfoSet.BeApplied;
                return false;
            }
        }

        public static bool RequireAmmo(RogueObj self, in RogueMethodArgument arg, out RogueObj ammo)
        {
            ammo = arg.Tool;
            if (ammo == null)
            {
                ammo = EquipmentUtility.GetAmmo(self, out _);
                if (ammo == null) return true;
            }
            if (SpaceUtility.ObjIsGlued(ammo))
            {
                return true;
            }

            return false;
        }

        public static bool RequireMatchedAmmo(
            RogueObj self, in RogueMethodArgument arg, Spanning<IKeyword> ammoCategories, out RogueObj ammo, out IAmmoEquipmentInfo ammoInfo)
        {
            ammoInfo = null;
            if (RequireAmmo(self, arg, out ammo)) return true;

            ammoInfo = EquipmentUtility.GetAmmoInfo(ammo);
            if (ammoInfo == null || !EquipmentUtility.MatchAmmo(ammoInfo, ammoCategories))
            {
                ammo = null;
                ammoInfo = null;
                return true;
            }

            return false;
        }
    }
}
