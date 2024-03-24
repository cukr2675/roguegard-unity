using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public class PartyBoardMenu : IModelsMenu
    {
        private static readonly ItemController itemController = new ItemController();
        private static readonly List<object> models = new List<object>();

        public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
        {
            // ���r�[�����o�[�̈ꗗ��\������
            var worldInfo = RogueWorldInfo.GetByCharacter(player);
            var lobbyMembers = worldInfo.LobbyMembers.Members;
            models.Clear();
            for (int i = 0; i < lobbyMembers.Count; i++)
            {
                if (lobbyMembers[i] == null) continue;

                models.Add(lobbyMembers[i]);
            }
            models.Add(null);

            var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
            scroll.OpenView(itemController, models, root, player, null, RogueMethodArgument.Identity);
            scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
        }

        private class ItemController : IModelsMenuItemController
        {
            private static readonly CommandMenu nextMenu = new CommandMenu();
            private static readonly CallLobbyDialog callLobbyDialog = new CallLobbyDialog();
            private static readonly PartyBoardCharacterCreationMenu newMenu = new PartyBoardCharacterCreationMenu();

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var character = (RogueObj)model;
                if (character == null)
                {
                    return "+ �ǉ�";
                }
                else
                {
                    var info = LobbyMemberList.GetMemberInfo(character);
                    var name = character.GetName();
                    if (info.Seat != null) return "<#ffff00>" + name; // �Ȃɂ��Ă���L�����͕ʃ��j���[
                    else return name;
                }
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var character = (RogueObj)model;
                if (character == null)
                {
                    // �V�K�����o�[�쐬
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                    var openArg = new RogueMethodArgument(other: builder);
                    root.OpenMenu(newMenu, self, user, openArg, openArg);
                }
                else
                {
                    // ���������o�[�̃��j���[�\��
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var info = LobbyMemberList.GetMemberInfo(character);
                    if (info.Seat != null)
                    {
                        // �Ȃɂ��Ă���L�����͂�������Ăі߂����q�˂�
                        root.AddInt(DeviceKw.StartTalk, 0);
                        root.AddObject(DeviceKw.AppendText, character);
                        root.AddObject(DeviceKw.AppendText, "���Ăі߂��܂����H");
                        root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                        root.OpenMenuAsDialog(callLobbyDialog, self, null, new(targetObj: character), arg);
                    }
                    else
                    {
                        root.OpenMenuAsDialog(nextMenu, self, null, new(targetObj: character), arg);
                    }
                }
            }
        }

        private class CallLobbyDialog : IModelsMenu
        {
            private static readonly object[] models = new object[]
            {
                new ActionModelsMenuChoice("�͂�", Yes),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
            }

            private static void Yes(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // �N�G�X�g�𒆎~���ăL������Ȃ���Ăі߂�
                var character = arg.TargetObj;
                var leader = character.Main.Stats.Party.Members[0];
                default(IActiveRogueMethodCaller).LocateSavePoint(leader, null, 0f, RogueWorldSavePointInfo.Instance, true);
                SpaceUtility.TryLocate(character, null);
                var info = LobbyMemberList.GetMemberInfo(character);
                info.Seat = null;

                root.Back();
            }
        }

        private class CommandMenu : IModelsMenu
        {
            private static readonly PartyBoardCharacterCreationMenu nextMenu = new PartyBoardCharacterCreationMenu();

            private static readonly object[] models = new object[]
            {
                new ActionModelsMenuChoice("���", Change),
                new ActionModelsMenuChoice("����", Invite),
                new ActionModelsMenuChoice("�ҏW", Edit),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var newPlayer = arg.TargetObj;
                var openArg = new RogueMethodArgument(targetObj: newPlayer);
                root.Get(DeviceKw.MenuCommand).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, null, openArg);
            }

            private static void Change(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // �Ȃ��ݒ肳��Ă���ꍇ�͎��s������
                var info = LobbyMemberList.GetMemberInfo(arg.TargetObj);
                if (info?.Seat != null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newPlayer = arg.TargetObj;

                // ��Ԉړ�
                var location = self.Location;
                var position = self.Position;
                SpaceUtility.TryLocate(self, null);
                SpaceUtility.TryLocate(newPlayer, location, position);
                newPlayer.Main.Stats.Direction = RogueDirection.Down;

                // �p�[�e�B�ړ�
                var party = self.Main.Stats.Party;
                self.Main.Stats.UnassignParty(self, party);
                newPlayer.Main.Stats.TryAssignParty(newPlayer, party);

                RogueDevice.Primary.AddObject(DeviceKw.ChangePlayer, newPlayer);

                root.Done();
            }

            private static void Invite(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // �Ȃ��ݒ肳��Ă���ꍇ�͎��s������
                var info = LobbyMemberList.GetMemberInfo(arg.TargetObj);
                if (info?.Seat != null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newMember = arg.TargetObj;

                // ��Ԉړ�
                var party = self.Main.Stats.Party;
                if (!default(IChangeStateRogueMethodCaller).LocateNextToAnyMember(newMember, self, 0f, party)) return;
                newMember.Main.Stats.Direction = RogueDirection.Down;

                // �p�[�e�B�ړ�
                newMember.Main.Stats.TryAssignParty(newMember, party);
                info.Seat = null;
                info.SavePoint = null;
                info.ItemRegister.Clear();
                var spaceObjs = newMember.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var item = spaceObjs[i];
                    if (item == null) continue;

                    info.ItemRegister.Add(item);
                }

                root.Done();
            }

            private static void Edit(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // �Ȃ��ݒ肳��Ă���ꍇ�͎��s������
                var info = LobbyMemberList.GetMemberInfo(arg.TargetObj);
                if (info?.Seat != null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                var character = arg.TargetObj;
                var builder = new CharacterCreationDataBuilder(info.CharacterCreationData);

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, self, user, new(targetObj: character, other: builder), new(targetObj: character, other: builder));
            }
        }
    }
}
