using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PartyBoardCharacterCreationMenu : RogueMenuScreen
    {
        private readonly ScrollViewTemplate<object, MMgr, MArg> view = new()
        {
            ScrollSubViewName = RoguegardSubViews.CharacterCreation,
            BackAnchorList = new()
            {
                null,
                SelectOption.Create<MMgr, MArg>("<", Save)
            },
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            // �v���Z�b�g�ǂݍ��݃{�^����ݒ肷��
            var characterCreation = RoguegardSubViews.GetCharacterCreation(manager);
            view.BackAnchorList[0] = characterCreation.LoadPresetOption;

            view.ShowTemplate(System.Array.Empty<object>(), manager, arg)
                ?
                .Build();
        }

        private static void Save(MMgr manager, MArg arg)
        {
            if (arg.Arg.Other is CharacterCreationDataBuilder builder)
            {
                // �L�����N����ʂ���߂����Ƃ��A���̃L�������X�V����
                var character = arg.Arg.TargetObj;
                if (character != null)
                {
                    // �ҏW�L�����X�V
                    character.Main.SetBaseInfoSet(character, builder.PrimaryInfoSet);
                }
                else
                {
                    // �V�K�L�����ǉ�
                    var worldInfo = RogueWorldInfo.GetByCharacter(arg.Self);
                    character = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                    worldInfo.LobbyMembers.Add(character);
                }
                var info = LobbyMemberList.GetMemberInfo(character);
                info.CharacterCreationData = builder;
            }

            manager.Back(2);
        }
    }
}
