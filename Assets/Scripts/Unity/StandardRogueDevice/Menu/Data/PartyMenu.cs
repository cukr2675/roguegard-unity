using Lysionium;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMenu : RogueListuiScreen
    {
        private readonly ScrollMenuViewData<RogueObj, MMgr> view = new()
        {
            Title = ":Party",
        };

        public PartyMenu(PartyMemberMenu memberMenu)
        {
            OnOpenScreen += (manager) =>
            {
                view.Show(Arg.Self.Main.Stats.Party.Members, manager)
                ?
                .OnClick((partyMember, manager) =>
                {
                    // 選択したパーティメンバーの情報と選択肢を表示する
                    manager.PushScreen(memberMenu, Arg.Self, targetObj: partyMember);
                })
                .Build();
            };
        }
    }
}
