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
        private readonly ScrollMenuViewData<RogueObj, MMgr, MArg> view = new()
        {
            Title = ":Party",
        };

        private readonly PartyMemberMenu memberMenu;

        public PartyMenu(PartyMemberMenu memberMenu)
        {
            this.memberMenu = memberMenu;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            view.Show(arg.Self.Main.Stats.Party.Members, manager, arg)
                ?
                .OnClick((partyMember, manager, arg) =>
                {
                    // 選択したパーティメンバーの情報と選択肢を表示する
                    manager.PushMenuScreen(memberMenu, arg.Self, targetObj: partyMember);
                })
                .Build();
        }
    }
}
