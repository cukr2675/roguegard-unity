using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMemberMenu : RogueMenuScreen
    {
        private readonly ObjsMenu objsMenu;
        private readonly ObjCommandMenu objCommandMenu;
        private readonly SkillsMenu skillsMenu;

        private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
        {
            PrimaryCommandSubViewName = StandardSubViewTable.SecondaryCommandName,
        };

        public override bool IsIncremental => true;

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu, SkillsMenu skillsMenu)
        {
            this.objsMenu = objsMenu;
            this.objCommandMenu = objCommandMenu;
            this.skillsMenu = skillsMenu;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            view.ShowTemplate(manager, arg)
                ?
                .Append(objCommandMenu.Summary)

                .Option(":Items", (manager, arg) =>
                {
                    manager.PushMenuScreen(objsMenu.Items, arg.Self, targetObj: arg.Self);
                })

                .Option(":Skills", (manager, arg) =>
                {
                    manager.PushMenuScreen(skillsMenu.Use, arg.Self);
                })

                .Back()

                .Build();
        }

        public override void CloseScreen(MMgr manager, bool back)
        {
            view.HideTemplate(manager, back);
        }
    }
}
