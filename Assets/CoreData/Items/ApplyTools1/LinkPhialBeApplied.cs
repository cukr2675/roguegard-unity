using Roguegard.CharacterCreation;
using Roguegard.Device;
using UnityEngine;

namespace Roguegard
{
    public class LinkPhialBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private CharacterCreationDataAsset _potionInfoSet = null;

        private readonly ObjSelectionScreen objSelectionScreen;

        private LinkPhialBeApplied()
        {
            var callback = new SecondDeviceCommand(this);
            objSelectionScreen = new ObjSelectionScreen(callback);
        }

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                RogueDevice.Primary.AddScreen(objSelectionScreen, user, null, RogueMethodArgument.Identity);
                return false;
            }
            else
            {
                return false;
            }
        }

        private class SecondDeviceCommand : IDeviceCommand
        {
            private readonly SelectedDeviceCommand callback;
            private readonly ObjSelectionScreen objSelectionScreen;

            public SecondDeviceCommand(LinkPhialBeApplied parent)
            {
                callback = new SelectedDeviceCommand() { parent = parent };
                objSelectionScreen = new ObjSelectionScreen(callback);
            }

            public bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (CommonAssert.RequireTool(arg, out var tool)) return false;

                callback.firstTool = tool;
                RogueDevice.Primary.AddScreen(objSelectionScreen, user, null, RogueMethodArgument.Identity);
                return true;
            }
        }

        private class SelectedDeviceCommand : IDeviceCommand
        {
            public LinkPhialBeApplied parent;
            public RogueObj firstTool;

            public bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
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
