using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMemberMenu : IListMenu
    {
        private readonly IListMenuSelectOption[] selectOptions;

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu, SkillsMenu skillsMenu)
        {
            selectOptions = new IListMenuSelectOption[]
            {
                objCommandMenu.Summary,
                new Objs() { nextMenu = objsMenu.Items },
                new Skill() { nextMenu = skillsMenu.Use },
                ExitListMenuSelectOption.Instance
            };
        }

        void IListMenu.OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.GetView(DeviceKw.MenuCommand).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);
        }

        private class Objs : BaseListMenuSelectOption
        {
            public override string Name => ":Items";

            public IListMenu nextMenu;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self);
                manager.OpenMenu(nextMenu, self, null, openArg);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Skill : BaseListMenuSelectOption
        {
            public override string Name => ":Skills";

            public IListMenu nextMenu;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }
    }
}
