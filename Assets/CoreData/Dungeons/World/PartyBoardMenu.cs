using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public class PartyBoardMenu : IListMenu
    {
        private static readonly Presenter presenter = new Presenter();
        private static readonly List<object> elms = new List<object>();

        public void OpenMenu(IListMenuManager manager, RogueObj player, RogueObj user, in RogueMethodArgument arg)
        {
            // ロビーメンバーの一覧を表示する
            var worldInfo = RogueWorldInfo.GetByCharacter(player);
            var lobbyMembers = worldInfo.LobbyMembers.Members;
            elms.Clear();
            for (int i = 0; i < lobbyMembers.Count; i++)
            {
                if (lobbyMembers[i] == null) continue;

                elms.Add(lobbyMembers[i]);
            }
            elms.Add(null);

            var scroll = manager.GetView(DeviceKw.MenuScroll);
            scroll.OpenView(presenter, elms, manager, player, null, RogueMethodArgument.Identity);
            ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
        }

        private class Presenter : IElementPresenter
        {
            private static readonly CommandMenu nextMenu = new CommandMenu();
            private static readonly DialogListMenuSelectOption callLobbyDialog = new DialogListMenuSelectOption(("はい", CallLobby)).AppendExit();
            private static readonly PartyBoardCharacterCreationMenu newMenu = new PartyBoardCharacterCreationMenu();

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var character = (RogueObj)element;
                if (character == null)
                {
                    return "+ 追加";
                }
                else
                {
                    var info = LobbyMemberList.GetMemberInfo(character);
                    var name = character.GetName();
                    if (info.Seat != null) return "<#ffff00>" + name; // 席についているキャラは別メニュー
                    else return name;
                }
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var character = (RogueObj)element;
                if (character == null)
                {
                    // 新規メンバー作成
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                    var openArg = new RogueMethodArgument(other: builder);
                    manager.OpenMenu(newMenu, self, user, openArg);
                }
                else
                {
                    // 既存メンバーのメニュー表示
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var info = LobbyMemberList.GetMemberInfo(character);
                    if (info.Seat != null)
                    {
                        // 席についているキャラはそこから呼び戻すか尋ねる
                        manager.AddInt(DeviceKw.StartTalk, 0);
                        manager.AddObject(DeviceKw.AppendText, character);
                        manager.AddObject(DeviceKw.AppendText, "を呼び戻しますか？");
                        manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
                        manager.OpenMenuAsDialog(callLobbyDialog, self, null, new(targetObj: character));
                    }
                    else
                    {
                        manager.OpenMenuAsDialog(nextMenu, self, null, new(targetObj: character));
                    }
                }
            }

            private static void CallLobby(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // クエストを中止してキャラを席から呼び戻す
                var character = arg.TargetObj;
                var leader = character.Main.Stats.Party.Members[0];
                default(IActiveRogueMethodCaller).LocateSavePoint(leader, null, 0f, RogueWorldSavePointInfo.Instance, true);
                SpaceUtility.TryLocate(character, null);
                var info = LobbyMemberList.GetMemberInfo(character);
                info.Seat = null;

                manager.Back();
            }
        }

        private class CommandMenu : IListMenu
        {
            private static readonly PartyBoardCharacterCreationMenu nextMenu = new PartyBoardCharacterCreationMenu();

            private static readonly object[] elms = new object[]
            {
                new ActionListMenuSelectOption("交代", Change),
                new ActionListMenuSelectOption("加入", Invite),
                new ActionListMenuSelectOption("編集", Edit),
                ExitListMenuSelectOption.Instance
            };

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var newPlayer = arg.TargetObj;
                var openArg = new RogueMethodArgument(targetObj: newPlayer);
                manager.GetView(DeviceKw.MenuCommand).OpenView(SelectOptionPresenter.Instance, elms, manager, self, null, openArg);
            }

            private static void Change(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newPlayer = arg.TargetObj;

                // 空間移動
                var location = self.Location;
                var position = self.Position;
                SpaceUtility.TryLocate(self, null);
                SpaceUtility.TryLocate(newPlayer, location, position);
                newPlayer.Main.Stats.Direction = RogueDirection.Down;

                // パーティ移動
                var party = self.Main.Stats.Party;
                self.Main.Stats.UnassignParty(self, party);
                newPlayer.Main.Stats.TryAssignParty(newPlayer, party);

                RogueDevice.Primary.AddObject(DeviceKw.ChangePlayer, newPlayer);

                manager.Done();
            }

            private static void Invite(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newMember = arg.TargetObj;

                // 空間移動
                var party = self.Main.Stats.Party;
                if (!default(IChangeStateRogueMethodCaller).LocateNextToAnyMember(newMember, self, 0f, party)) return;
                newMember.Main.Stats.Direction = RogueDirection.Down;

                // パーティ移動
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

            private static void Edit(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                var character = arg.TargetObj;
                var builder = new CharacterCreationDataBuilder(info.CharacterCreationData);

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.OpenMenu(nextMenu, self, user, new(targetObj: character, other: builder));
            }
        }
    }
}
