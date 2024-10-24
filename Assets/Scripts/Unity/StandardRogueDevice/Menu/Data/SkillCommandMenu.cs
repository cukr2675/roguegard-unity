using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class SkillCommandMenu : RogueMenuScreen
    {
        public override bool IsIncremental => true;

        private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
            PrimaryCommandSubViewName = StandardSubViewTable.SecondaryCommandName,
        };

        private readonly CommandAction commandAction = new();

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            var selectedSkill = (ISkill)arg.Arg.Other;

            view.Title = selectedSkill.Details.ToString();

            view.Show(manager, arg)
                ?.Option(":Use", (manager, arg) =>
                {
                    var info = RogueDeviceEffect.Get(arg.Self);
                    var selectedSkill = (ISkill)arg.Arg.Other;
                    info.SetDeviceCommand(commandAction, null, new(other: selectedSkill));
                    manager.Done();
                })
                .Exit()
                .Build();
        }

        public override void CloseScreen(RogueMenuManager manager, bool back)
        {
            view.HideSubViews(manager, back);
        }

        private class CommandAction : IDeviceCommandAction
        {
            public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
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
