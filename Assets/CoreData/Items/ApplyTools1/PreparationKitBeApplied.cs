using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PreparationKitBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private ScriptableCharacterCreationData _potionInfoSet = null;

        private readonly SelectObjMenu menu;

        private PreparationKitBeApplied()
        {
            var callback = new SelectedRogueMethod() { parent = this };
            menu = new SelectObjMenu(callback);
        }

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                RogueDevice.Primary.AddMenu(menu, user, null, RogueMethodArgument.Identity);
                return false;
            }
            else
            {
                return false;
            }
        }

        private class SelectedRogueMethod : IDeviceCommandAction
        {
            public PreparationKitBeApplied parent;

            public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (CommonAssert.RequireTool(arg, out var tool)) return false;

                // スタックしていたら一つ調合する。
                if (tool.Stack >= 2)
                {
                    if (!SpaceUtility.TryDividedLocate(arg.Tool, 1, out tool)) return false;
                }
                InfoSetReferenceInfo.SetTo(tool, tool.Main.InfoSet);
                tool.Main.SetBaseInfoSet(tool, parent._potionInfoSet.PrimaryInfoSet);
                if (RogueDevice.Primary.Player == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を調合してポーションにした\n");
                }
                return true;
            }
        }
    }
}
