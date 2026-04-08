using Lysionium;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class SkillCommandMenuScreen : RogueListuiScreen
    {
        private readonly MainMenuViewData<MMgr> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        public SkillCommandMenuScreen()
        {
            OnOpenScreen += (manager) =>
            {
                var selectedSkill = (ISkill)Arg.Arg.Other;

                view.Title = StandardRogueDeviceUtility.GetCaption(selectedSkill);

                view.Show(manager)
                ?
                .VarOnce(out var deviceCommand, new DeviceCommand())
                .Option(":Use", (manager) =>
                {
                    var info = RogueDeviceEffect.Get(Arg.Self);
                    var selectedSkill = (ISkill)Arg.Arg.Other;
                    info.SetDeviceCommand(deviceCommand, null, new(other: selectedSkill));
                    manager.Done();
                })
                .Back()
                .Build();
            };

            OnCloseScreenView += (manager, back) =>
            {
                view.Hide(manager, back);
            };
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
