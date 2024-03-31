using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMemberMenu : IModelsMenu
    {
        private readonly IModelsMenuChoice[] choices;

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu, SkillsMenu skillsMenu)
        {
            choices = new IModelsMenuChoice[]
            {
                objCommandMenu.Summary,
                new Objs() { nextMenu = objsMenu.Items },
                new Skill() { nextMenu = skillsMenu.Use },
                ExitModelsMenuChoice.Instance
            };
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuCommand).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, arg);
        }

        private class Objs : BaseModelsMenuChoice
        {
            public override string Name => ":Items";

            public IModelsMenu nextMenu;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self);
                root.OpenMenu(nextMenu, self, null, openArg);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Skill : BaseModelsMenuChoice
        {
            public override string Name => ":Skills";

            public IModelsMenu nextMenu;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }
    }
}
