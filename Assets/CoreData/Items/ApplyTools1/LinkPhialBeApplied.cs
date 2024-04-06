using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class LinkPhialBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private ScriptableCharacterCreationData _potionInfoSet = null;

        private readonly SelectObjMenu menu;

        private LinkPhialBeApplied()
        {
            var callback = new SecondRogueMethod(this);
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

        private class SecondRogueMethod : IDeviceCommandAction
        {
            private readonly SelectedRogueMethod callback;
            private readonly SelectObjMenu menu;

            public SecondRogueMethod(LinkPhialBeApplied parent)
            {
                callback = new SelectedRogueMethod() { parent = parent };
                menu = new SelectObjMenu(callback);
            }

            public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (CommonAssert.RequireTool(arg, out var tool)) return false;

                callback.firstTool = tool;
                RogueDevice.Primary.AddMenu(menu, user, null, RogueMethodArgument.Identity);
                return true;
            }
        }

        private class SelectedRogueMethod : IDeviceCommandAction
        {
            public LinkPhialBeApplied parent;
            public RogueObj firstTool;

            public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (CommonAssert.RequireTool(arg, out var secondTool)) return false;

                // スタックしていても一つだけ調合する。
                if (firstTool.Stack >= 2)
                {
                    if (!SpaceUtility.TryDividedLocate(firstTool, 1, out firstTool)) return false;
                }
                if (secondTool.Stack >= 2)
                {
                    if (!SpaceUtility.TryDividedLocate(secondTool, 1, out secondTool)) return false;
                }

                InfoSetReferenceInfo.SetTo(firstTool, firstTool.Main.InfoSet, secondTool.Main.InfoSet);
                firstTool.Main.SetBaseInfoSet(firstTool, parent._potionInfoSet.PrimaryInfoSet);
                if (RogueDevice.Primary.Player == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, firstTool);
                    RogueDevice.Add(DeviceKw.AppendText, "と");
                    RogueDevice.Add(DeviceKw.AppendText, secondTool);
                    RogueDevice.Add(DeviceKw.AppendText, "を連結ビンに入れた\n");
                }
                return true;
            }
        }
    }
}
