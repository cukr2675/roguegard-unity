using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class SkillCommandMenu : IModelsMenu
    {
        private readonly IModelsMenuChoice[] choices;
        private ISkill currentSkill;

        public SkillCommandMenu()
        {
            choices = new IModelsMenuChoice[]
            {
                new UseChoice() { parent = this },
                ExitModelsMenuChoice.Instance
            };
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            currentSkill = (ISkill)arg.Other;
            root.Get(DeviceKw.MenuCommand).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, arg);

            var caption = root.Get(DeviceKw.MenuCaption);
            caption.OpenView(null, Spanning<object>.Empty, null, null, null, new(other: currentSkill));
        }

        private class UseChoice : IModelsMenuChoice, IDeviceCommandAction
        {
            public SkillCommandMenu parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "使う";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var info = RogueDeviceEffect.Get(self);
                info.SetDeviceCommand(this, null, RogueMethodArgument.Identity);
                root.Done();
            }

            bool IDeviceCommandAction.CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (!RoguegardSettings.KeywordsNotEnqueueMessageRule.Contains(MainInfoKw.Skill))
                {
                    RogueDevice.Add(DeviceKw.AppendText, DeviceKw.HorizontalRule);
                }

                return RogueMethodAspectState.Invoke(MainInfoKw.Skill, parent.currentSkill, self, user, activationDepth, arg);
            }
        }
    }
}
