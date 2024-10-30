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
        private readonly ScrollViewTemplate<object, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
            ScrollSubViewName = RogueMenuManager.CharacterCreationName,
            BackAnchorList = new()
            {
                null,
                ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("<", Save)
            },
        };

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            view.BackAnchorList[0] = manager.LoadPresetSelectOptionOfCharacterCreation;

            view.ShowTemplate(System.Array.Empty<object>(), manager, arg)
                ?
                .Build();
        }

        private static void Save(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

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

            manager.HandleClickBack();
            manager.HandleClickBack();
        }
    }
}
