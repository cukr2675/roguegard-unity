using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PartyBoardCharacterCreationMenu : IModelsMenu
    {
        private readonly object[] models = new object[] { new OpenExitDialog() };

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var builder = (CharacterCreationDataBuilder)arg.Other;
            root.Get(DeviceKw.MenuCharacterCreation).OpenView(null, models, root, self, user, new(targetObj: arg.TargetObj, other: builder));
        }

        private class OpenExitDialog : IModelsMenuChoice
        {
            private static readonly ExitDialog nextMenu = new ExitDialog();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "<";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddInt(DeviceKw.StartTalk, 0);
                root.AddObject(DeviceKw.AppendText, "�ҏW���e��ۑ����܂����H");
                root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                root.OpenMenuAsDialog(nextMenu, self, user, arg, arg);
            }
        }

        private class ExitDialog : IModelsMenu
        {
            private readonly object[] models = new object[]
            {
                new ActionModelsMenuChoice("�㏑���ۑ�", Save),
                new ActionModelsMenuChoice("�ۑ����Ȃ�", NotSave),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
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
                        var world = RogueWorld.GetWorld(player);
                        character = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                        LobbyMembers.Add(character, world);
                    }
                    var info = LobbyMembers.GetMemberInfo(character);
                    info.CharacterCreationData = builder;
                }

                root.Back();
                root.Back();
            }

            private static void NotSave(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // ������������
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Back();
                root.Back();
            }
        }
    }
}
