using Lysionium;
using Lysionium.MergeExtensions.R3;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public class PartyBoardScreen : RogueListuiScreen
    {
        private readonly ScrollMenuViewData<RogueObj, MMgr> view = new()
        {
        };

        public PartyBoardScreen()
        {
            OnOpenScreen += (manager) =>
            {
                // ロビーメンバーの一覧を表示する
                var worldInfo = RogueWorldInfo.GetByCharacter(Arg.Self);

                view.Show(worldInfo.LobbyMembers.Members, manager)
                ?
                .Merge(out var merged)
                .Init(
                    () => merged
                    .Filter(lobbyMember => lobbyMember != null)

                    .Case(
                        lobbyMember => LobbyMemberList.GetMemberInfo(lobbyMember).Seat != null,
                        _ => _
                        .NameFrom(lobbyMember => "<#ffff00>" + lobbyMember.GetName())

                        // 席についているキャラはそこから呼び戻すか尋ねる
                        .VarOnce(
                            out var callLobbyDialog, new ChoicesScreen(_ => $"{Arg.Arg.TargetObj}を呼び戻しますか？")
                            .Option("はい", CallLobby)
                            .Back())
                        .OnClick((lobbyMember, manager) => manager.PushScreen(callLobbyDialog, Arg.Self, targetObj: lobbyMember)))

                    .Otherwise(
                        _ => _
                        .NameFrom(lobbyMember => lobbyMember.GetName())
                        .VarOnce(out var nextScreen, new CommandMenuScreen())
                        .OnClick((lobbyMember, manager) => manager.PushScreen(nextScreen, Arg.Self, targetObj: lobbyMember))))

                .VarOnce(out var creationScreen, new PartyBoardCharacterCreationScreen())
                .Tail.Option("+ 追加", (manager) =>
                {
                    // 新規メンバー作成
                    var characterCreationData = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                    manager.PushScreen(creationScreen, Arg.Self, Arg.User, other: characterCreationData);
                })

                .Build();
            };
        }

        private void CallLobby(MMgr manager)
        {
            // クエストを中止してキャラを席から呼び戻す
            var character = Arg.Arg.TargetObj;
            var leader = character.Main.Stats.Party.Members[0];
            default(IActiveRogueMethodCaller).LocateSavePoint(leader, null, 0f, RogueWorldSavePointInfo.Instance, true);
            SpaceUtility.TryLocate(character, null);
            var info = LobbyMemberList.GetMemberInfo(character);
            info.Seat = null;

            manager.PopScreen();
        }

        private class CommandMenuScreen : RogueListuiScreen
        {
            private readonly MainMenuViewData<MMgr> view = new()
            {
                PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
            };

            public CommandMenuScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(manager)
                    ?
                    .VarOnce(out var nextScreen, new PartyBoardCharacterCreationScreen())
                    .Option("交代", Change)
                    .Option("加入", Invite)
                    .Option("編集", m => Edit(m, nextScreen))
                    .Back()
                    .Build();
                };

                OnCloseScreenView += (manager, back) =>
                {
                    view.Hide(manager, back);
                };
            }

            /// <summary>
            /// 交代ボタンクリック時
            /// </summary>
            private void Change(MMgr manager)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(Arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newPlayer = Arg.Arg.TargetObj;

                // 空間移動
                var self = Arg.Self;
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

            /// <summary>
            /// 加入ボタンクリック時
            /// </summary>
            private void Invite(MMgr manager)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(Arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newMember = Arg.Arg.TargetObj;

                // 空間移動
                var party = Arg.Self.Main.Stats.Party;
                if (!default(IChangeStateRogueMethodCaller).LocateNextToAnyMember(newMember, Arg.Self, 0f, party)) return;
                newMember.Main.Stats.Direction = RogueDirection.Down;

                // パーティ移動
                newMember.Main.Stats.TryAssignParty(newMember, party);
                info.Seat = null;
                info.SavePoint = null;
                info.ItemRegister.Clear();
                foreach (var item in newMember.Space.Objs)
                {
                    if (item == null) continue;

                    info.ItemRegister.Add(item);
                }

                manager.Done();
            }

            /// <summary>
            /// 編集ボタンクリック時
            /// </summary>
            private void Edit(MMgr manager, PartyBoardCharacterCreationScreen nextScreen)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(Arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                var character = Arg.Arg.TargetObj;
                var characterCreationData = new CharacterCreationData(info.CharacterCreationData);

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.PushScreen(nextScreen, Arg.Self, Arg.User, targetObj: character, other: characterCreationData);
            }
        }
    }
}
