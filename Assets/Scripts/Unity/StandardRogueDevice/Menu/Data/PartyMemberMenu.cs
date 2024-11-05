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

        private readonly CommandListViewTemplate<ISelectOption, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu, SkillsMenu skillsMenu)
        {
            selectOptions = new ISelectOption[]
            {
                objCommandMenu.Summary,
                SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Items", (manager, arg) =>
                {
                    manager.PushMenuScreen(objsMenu.Items, arg.Self, targetObj: arg.Self);
                }),
                SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Skills", (manager, arg) =>
                {
                    manager.PushMenuScreen(skillsMenu.Use, arg.Self);
                }),
                BackSelectOption.Instance
            };
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            view.ShowTemplate(selectOptions, manager, arg)
                ?.Build();
        }
    }
}
