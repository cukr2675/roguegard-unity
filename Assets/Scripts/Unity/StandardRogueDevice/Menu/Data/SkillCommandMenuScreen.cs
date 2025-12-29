using Lysionium;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class SkillCommandMenuScreen : RogueListuiScreen
    {
        public override bool IsIncremental => true;

        private readonly MainMenuViewData<MMgr, MArg> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        private readonly DeviceCommand deviceCommand = new();

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            var selectedSkill = (ISkill)arg.Arg.Other;

            view.Title = StandardRogueDeviceUtility.GetCaption(selectedSkill);

            view.Show(manager, arg)
                ?.Option(":Use", (manager, arg) =>
                {
                    var info = RogueDeviceEffect.Get(arg.Self);
                    var selectedSkill = (ISkill)arg.Arg.Other;
                    info.SetDeviceCommand(deviceCommand, null, new(other: selectedSkill));
                    manager.Done();
                })
                .Back()
                .Build();
        }

        public override void CloseScreenView(MMgr manager, bool back)
        {
            view.Hide(manager, back);
        }

        private class DeviceCommand : IDeviceCommand
        {
            public bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (!RoguegardSettings.KeywordsNotEnqueueMessageRule.Contains(MainInfoKw.Skill))
                {
                    RogueDevice.Add(DeviceKw.AppendText, DeviceKw.HorizontalRule);
                }

                var selectedSkill = (ISkill)arg.Other;
                return RogueMethodAspectState.Invoke(MainInfoKw.Skill, selectedSkill, self, user, activationDepth, RogueMethodArgument.Identity);
            }
        }
    }
}
