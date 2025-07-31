using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMenu : RogueMenuScreen
    {
        private readonly PartyMemberMenu memberMenu;

        private readonly List<RogueObj> partyMembers = new();

        private readonly ScrollViewData<RogueObj, MMgr, MArg> view = new()
        {
            Title = ":Party",
        };

        public PartyMenu(PartyMemberMenu memberMenu)
        {
            this.memberMenu = memberMenu;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            partyMembers.Clear();
            foreach (var partyMemberObj in arg.Self.Main.Stats.Party.Members)
            {
                partyMembers.Add(partyMemberObj);
            }

            view.Show(partyMembers, manager, arg)
                ?.OnClick((partyMember, manager, arg) =>
                {
                    // 選択したパーティメンバーの情報と選択肢を表示する
                    manager.PushMenuScreen(memberMenu, arg.Self, targetObj: partyMember);
                })
                .Build();
        }
    }
}
