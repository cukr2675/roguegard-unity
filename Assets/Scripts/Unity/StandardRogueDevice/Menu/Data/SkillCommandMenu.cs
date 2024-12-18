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

        private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
        {
            PrimaryCommandSubViewName = StandardSubViewTable.SecondaryCommandName,
        };

        private readonly CommandAction commandAction = new();

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            var selectedSkill = (ISkill)arg.Arg.Other;

            view.Title = CaptionWindow.ShowCaption(selectedSkill);

            view.ShowTemplate(manager, arg)
                ?.Option(":Use", (manager, arg) =>
                {
                    var info = RogueDeviceEffect.Get(arg.Self);
                    var selectedSkill = (ISkill)arg.Arg.Other;
                    info.SetDeviceCommand(commandAction, null, new(other: selectedSkill));
                    manager.Done();
                })
                .Back()
                .Build();
        }

        public override void CloseScreen(MMgr manager, bool back)
        {
            view.HideTemplate(manager, back);
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
