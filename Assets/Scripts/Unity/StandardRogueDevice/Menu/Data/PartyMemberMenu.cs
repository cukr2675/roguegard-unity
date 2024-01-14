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

        public PartyMemberMenu(CaptionWindow captionWindow, ObjsMenu objsMenu, ObjCommandMenu objCommandMenu, SkillsMenu skillsMenu)
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

        private class Objs : IModelsMenuChoice
        {
            public IModelsMenu nextMenu;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "道具";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self);
                root.OpenMenu(nextMenu, self, null, openArg, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Skill : IModelsMenuChoice
        {
            public IModelsMenu nextMenu;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "スキル";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }
    }
}
