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
            // ロビーメンバーの一覧を表示する
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

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var character = (RogueObj)model;
                if (character == null)
                {
                    // 新規メンバー作成
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                    var openArg = new RogueMethodArgument(other: builder);
                    root.OpenMenu(newMenu, self, user, openArg, openArg);
                }
                else
                {
                    // 既存メンバーのメニュー表示
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var info = LobbyMemberList.GetMemberInfo(character);
                    if (info.Seat != null)
                    {
                        // 席についているキャラはそこから呼び戻すか尋ねる
                        root.AddInt(DeviceKw.StartTalk, 0);
                        root.AddObject(DeviceKw.AppendText, character);
                        root.AddObject(DeviceKw.AppendText, "を呼び戻しますか？");
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
                new ActionModelsMenuChoice("はい", Yes),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
            }

            private static void Yes(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // キャラを席から呼び戻す
                var character = arg.TargetObj;
                default(IActiveRogueMethodCaller).LocateSavePoint(character, null, 0f, RogueWorldSavePointInfo.Instance, true);
                SpaceUtility.TryLocate(character, null);
                var info = LobbyMemberList.GetMemberInfo(character);
                info.Seat = null;

                root.Back();
            }
        }

        private class CommandMenu : IModelsMenu
        {
            private static readonly object[] models = new object[]
            {
                new Change(),
                new Edit(),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var newPlayer = arg.TargetObj;
                var openArg = new RogueMethodArgument(targetObj: newPlayer);
                root.Get(DeviceKw.MenuCommand).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, null, openArg);
            }
        }

        private class Change : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 席が設定されている場合はグレーアウト
                var info = LobbyMemberList.GetMemberInfo(arg.TargetObj);
                if (info?.Seat != null) return "<#808080>交代";
                else return "交代";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var info = LobbyMemberList.GetMemberInfo(arg.TargetObj);
                if (info?.Seat != null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
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

                root.Done();
            }
        }

        private class Edit : IModelsMenuChoice
        {
            private static readonly PartyBoardCharacterCreationMenu nextMenu = new PartyBoardCharacterCreationMenu();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "編集";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var character = arg.TargetObj;
                var info = LobbyMemberList.GetMemberInfo(character);
                var builder = new CharacterCreationDataBuilder(info.CharacterCreationData);

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, self, user, new(targetObj: character, other: builder), new(targetObj: character, other: builder));
            }
        }
    }
}
