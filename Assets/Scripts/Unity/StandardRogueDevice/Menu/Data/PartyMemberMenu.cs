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
        private readonly IListMenuSelectOption[] selectOptions;

        private readonly CommandListViewTemplate<IListMenuSelectOption, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu, SkillsMenu skillsMenu)
        {
            selectOptions = new IListMenuSelectOption[]
            {
                objCommandMenu.Summary,
                ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Items", (manager, arg) =>
                {
                    manager.PushMenuScreen(objsMenu.Items, arg.Self, targetObj: arg.Self);
                }),
                ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Skills", (manager, arg) =>
                {
                    manager.PushMenuScreen(skillsMenu.Use, arg.Self);
                }),
                ExitListMenuSelectOption.Instance
            };
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            view.Show(selectOptions, manager, arg)
                ?.Build();
        }
    }
}
