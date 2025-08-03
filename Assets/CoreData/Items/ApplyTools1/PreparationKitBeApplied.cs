using Roguegard.CharacterCreation;
using Roguegard.Device;
using UnityEngine;

namespace Roguegard
{
    public class PreparationKitBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private CharacterCreationDataAsset _potionInfoSet = null;

        private readonly SelectObjMenuScreen menu;

        private PreparationKitBeApplied()
        {
            var callback = new SelectedDeviceCommand() { parent = this };
            menu = new SelectObjMenuScreen(callback);
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

        private class SelectedDeviceCommand : IDeviceCommand
        {
            public PreparationKitBeApplied parent;

            public bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
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
