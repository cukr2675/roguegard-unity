using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMenu : BaseScrollModelsMenu<RogueObj>
    {
        protected override string MenuName => ":Party";

        private readonly PartyMemberMenu memberMenu;

        public PartyMenu(PartyMemberMenu memberMenu)
        {
            this.memberMenu = memberMenu;
        }

        protected override Spanning<RogueObj> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var partyMembers = self.Main.Stats.Party.Members;
            return partyMembers;
        }

        protected override string GetItemName(RogueObj partyMember, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return partyMember.GetName();
        }

        protected override void ItemActivate(RogueObj partyMember, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            // �I�������p�[�e�B�����o�[�̏��ƑI������\������
            root.OpenMenuAsDialog(memberMenu, self, null, new(targetObj: partyMember), arg);
        }
    }
}
