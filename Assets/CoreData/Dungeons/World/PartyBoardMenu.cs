using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public class PartyBoardMenu : RogueMenuScreen
    {
        private static readonly List<RogueObj> elms = new();

        private static readonly CommandMenu nextMenu = new CommandMenu();
        private static readonly PartyBoardCharacterCreationMenu newMenu = new PartyBoardCharacterCreationMenu();

        private readonly ScrollViewTemplate<RogueObj, MMgr, MArg> view = new()
        {
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            // ���r�[�����o�[�̈ꗗ��\������
            var worldInfo = RogueWorldInfo.GetByCharacter(arg.Self);
            var lobbyMembers = worldInfo.LobbyMembers.Members;
            elms.Clear();
            for (int i = 0; i < lobbyMembers.Count; i++)
            {
                if (lobbyMembers[i] == null) continue;

                elms.Add(lobbyMembers[i]);
            }
            elms.Add(null);

            view.ShowTemplate(elms, manager, arg)
                ?
                
                .ElementNameFrom((obj, manager, arg) =>
                {
                    if (obj == null)
                    {
                        return "+ �ǉ�";
                    }
                    else
                    {
                        var info = LobbyMemberList.GetMemberInfo(obj);
                        var name = obj.GetName();
                        if (info.Seat != null) return "<#ffff00>" + name; // �Ȃɂ��Ă���L�����͕ʃ��j���[
                        else return name;
                    }
                })

                .VariableOnce(out ChoicesMenuScreen callLobbyDialog)
                .OnClickElement((obj, manager, arg) =>
                {
                    if (obj == null)
                    {
                        // �V�K�����o�[�쐬
                        var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                        manager.PushMenuScreen(newMenu, arg.Self, arg.User, other: builder);
                    }
                    else
                    {
                        // ���������o�[�̃��j���[�\��
                        var info = LobbyMemberList.GetMemberInfo(obj);
                        if (info.Seat != null)
                        {
                            // �Ȃɂ��Ă���L�����͂�������Ăі߂����q�˂�
                            callLobbyDialog ??= new ChoicesMenuScreen(
                                (manager, arg) => $"{arg.Arg.TargetObj}���Ăі߂��܂����H")
                            .Option("�͂�", CallLobby)
                            .Back();

                            manager.PushMenuScreen(callLobbyDialog, arg.Self, targetObj: obj);
                        }
                        else
                        {
                            manager.PushMenuScreen(nextMenu, arg.Self, targetObj: obj);
                        }
                    }
                })

                .Build();
        }

        private static void CallLobby(MMgr manager, MArg arg)
        {
            // �N�G�X�g�𒆎~���ăL������Ȃ���Ăі߂�
            var character = arg.Arg.TargetObj;
            var leader = character.Main.Stats.Party.Members[0];
            default(IActiveRogueMethodCaller).LocateSavePoint(leader, null, 0f, RogueWorldSavePointInfo.Instance, true);
            SpaceUtility.TryLocate(character, null);
            var info = LobbyMemberList.GetMemberInfo(character);
            info.Seat = null;

            manager.Back();
        }

        private class CommandMenu : RogueMenuScreen
        {
            private static readonly PartyBoardCharacterCreationMenu nextMenu = new PartyBoardCharacterCreationMenu();

            private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(manager, arg)
                    ?.Option("���", Change)
                    .Option("����", Invite)
                    .Option("�ҏW", Edit)
                    .Back()
                    .Build();
            }

            private static void Change(MMgr manager, MArg arg)
            {
                // �Ȃ��ݒ肳��Ă���ꍇ�͎��s������
                var info = LobbyMemberList.GetMemberInfo(arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newPlayer = arg.Arg.TargetObj;

                // ��Ԉړ�
                var self = arg.Self;
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

                manager.Done();
            }

            private static void Invite(MMgr manager, MArg arg)
            {
                // �Ȃ��ݒ肳��Ă���ꍇ�͎��s������
                var info = LobbyMemberList.GetMemberInfo(arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newMember = arg.Arg.TargetObj;

                // ��Ԉړ�
                var party = arg.Self.Main.Stats.Party;
                if (!default(IChangeStateRogueMethodCaller).LocateNextToAnyMember(newMember, arg.Self, 0f, party)) return;
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

                manager.Done();
            }

            private static void Edit(MMgr manager, MArg arg)
            {
                // �Ȃ��ݒ肳��Ă���ꍇ�͎��s������
                var info = LobbyMemberList.GetMemberInfo(arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                var character = arg.Arg.TargetObj;
                var builder = new CharacterCreationDataBuilder(info.CharacterCreationData);

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.PushMenuScreen(nextMenu, arg.Self, arg.User, targetObj: character, other: builder);
            }
        }
    }
}
