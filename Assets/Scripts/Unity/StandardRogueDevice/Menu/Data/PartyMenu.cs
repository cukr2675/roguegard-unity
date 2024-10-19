using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMenu : RogueMenuScreen
    {
        private readonly PartyMemberMenu memberMenu;

        private readonly List<RogueObj> partyMembers = new();

        private readonly ScrollViewTemplate<RogueObj, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
            Title = ":Party",
        };

        public PartyMenu(PartyMemberMenu memberMenu)
        {
            this.memberMenu = memberMenu;
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            var partyMemberObjs = arg.Self.Main.Stats.Party.Members;
            partyMembers.Clear();
            for (int i = 0; i < partyMemberObjs.Count; i++)
            {
                partyMembers.Add(partyMemberObjs[i]);
            }

            view.Show(partyMembers, manager, arg)
                ?.OnClickElement((partyMember, manager, arg) =>
                {
                    // 選択したパーティメンバーの情報と選択肢を表示する
                    manager.PushMenuScreen(memberMenu, arg.Self, targetObj: partyMember);
                })
                .Build();
        }
    }
}
