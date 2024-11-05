using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMemberMenu : RogueMenuScreen
    {
        private readonly ISelectOption[] selectOptions;

        private readonly CommandListViewTemplate<ISelectOption, MMgr, MArg> view = new()
        {
        };

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu, SkillsMenu skillsMenu)
        {
            selectOptions = new ISelectOption[]
            {
                objCommandMenu.Summary,
                SelectOption.Create<MMgr, MArg>(":Items", (manager, arg) =>
                {
                    manager.PushMenuScreen(objsMenu.Items, arg.Self, targetObj: arg.Self);
                }),
                SelectOption.Create<MMgr, MArg>(":Skills", (manager, arg) =>
                {
                    manager.PushMenuScreen(skillsMenu.Use, arg.Self);
                }),
                BackSelectOption.Instance
            };
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            view.ShowTemplate(selectOptions, manager, arg)
                ?.Build();
        }
    }
}
