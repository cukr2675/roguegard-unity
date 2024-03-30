using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PartyBoardCharacterCreationMenu : IModelsMenu
    {
        private readonly object[] models = new object[] { DialogModelsMenuChoice.CreateExit(Save) };

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var builder = (CharacterCreationDataBuilder)arg.Other;
            root.Get(DeviceKw.MenuCharacterCreation).OpenView(null, models, root, self, user, new(targetObj: arg.TargetObj, other: builder));
        }

        private static void Save(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            if (arg.Other is CharacterCreationDataBuilder builder)
            {
                // �L�����N����ʂ���߂����Ƃ��A���̃L�������X�V����
                var character = arg.TargetObj;
                if (character != null)
                {
                    // �ҏW�L�����X�V
                    character.Main.SetBaseInfoSet(character, builder.PrimaryInfoSet);
                }
                else
                {
                    // �V�K�L�����ǉ�
                    var worldInfo = RogueWorldInfo.GetByCharacter(player);
                    character = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                    worldInfo.LobbyMembers.Add(character);
                }
                var info = LobbyMemberList.GetMemberInfo(character);
                info.CharacterCreationData = builder;
            }

            root.Back();
            root.Back();
        }
    }
}
