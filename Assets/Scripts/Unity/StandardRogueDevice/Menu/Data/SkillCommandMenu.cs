using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class SkillCommandMenu : IListMenu
    {
        private readonly IListMenuSelectOption[] selectOptions;
        private ISkill currentSkill;

        public SkillCommandMenu()
        {
            selectOptions = new IListMenuSelectOption[]
            {
                new UseSelectOption() { parent = this },
                ExitListMenuSelectOption.Instance
            };
        }

        void IListMenu.OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            currentSkill = (ISkill)arg.Other;
            manager.GetView(DeviceKw.MenuCommand).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);

            var caption = manager.GetView(DeviceKw.MenuCaption);
            caption.OpenView(null, Spanning<object>.Empty, null, null, null, new(other: currentSkill));
        }

        private class UseSelectOption : BaseListMenuSelectOption, IDeviceCommandAction
        {
            public override string Name => "使う";

            public SkillCommandMenu parent;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var info = RogueDeviceEffect.Get(self);
                info.SetDeviceCommand(this, null, RogueMethodArgument.Identity);
                manager.Done();
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
