using Lysionium;
using Lysionium.MergeExtensions.R3;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public class PartyBoardMenu : RogueMenuScreen
    {
        private readonly ScrollMenuViewData<RogueObj, MMgr, MArg> view = new()
        {
        };

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            // ロビーメンバーの一覧を表示する
            var worldInfo = RogueWorldInfo.GetByCharacter(arg.Self);

            view.Show(worldInfo.LobbyMembers.Members, manager, arg)
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
                            out var callLobbyDialog, new ChoicesMenuScreen((manager, arg) => $"{arg.Arg.TargetObj}を呼び戻しますか？")
                            .Option("はい", CallLobby)
                            .Back())
                        .OnClick((lobbyMember, manager, arg) => manager.PushMenuScreen(callLobbyDialog, arg.Self, targetObj: lobbyMember)))

                    .Otherwise(
                        _ => _
                        .NameFrom(lobbyMember => lobbyMember.GetName())
                        .VarOnce(out var nextMenu, new CommandMenu())
                        .OnClick((lobbyMember, manager, arg) => manager.PushMenuScreen(nextMenu, arg.Self, targetObj: lobbyMember))))

                .VarOnce(out var newMenu, new PartyBoardCharacterCreationMenu())
                .Tail.Option("+ 追加", (manager, arg) =>
                {
                    // 新規メンバー作成
                    var characterCreationData = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                    manager.PushMenuScreen(newMenu, arg.Self, arg.User, other: characterCreationData);
                })

                .Build();
        }

        private static void CallLobby(MMgr manager, MArg arg)
        {
            // クエストを中止してキャラを席から呼び戻す
            var character = arg.Arg.TargetObj;
            var leader = character.Main.Stats.Party.Members[0];
            default(IActiveRogueMethodCaller).LocateSavePoint(leader, null, 0f, RogueWorldSavePointInfo.Instance, true);
            SpaceUtility.TryLocate(character, null);
            var info = LobbyMemberList.GetMemberInfo(character);
            info.Seat = null;

            manager.PopMenuScreen();
        }

        private class CommandMenu : RogueMenuScreen
        {
            private static readonly PartyBoardCharacterCreationMenu nextMenu = new();

            private readonly MainMenuViewData<MMgr, MArg> view = new()
            {
                PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                view.Show(manager, arg)
                    ?
                    .Option("交代", Change)
                    .Option("加入", Invite)
                    .Option("編集", Edit)
                    .Back()
                    .Build();
            }

            /// <summary>
            /// 交代ボタンクリック時
            /// </summary>
            private static void Change(MMgr manager, MArg arg)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newPlayer = arg.Arg.TargetObj;

                // 空間移動
                var self = arg.Self;
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
            private static void Invite(MMgr manager, MArg arg)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var newMember = arg.Arg.TargetObj;

                // 空間移動
                var party = arg.Self.Main.Stats.Party;
                if (!default(IChangeStateRogueMethodCaller).LocateNextToAnyMember(newMember, arg.Self, 0f, party)) return;
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
            private static void Edit(MMgr manager, MArg arg)
            {
                // 席が設定されている場合は失敗させる
                var info = LobbyMemberList.GetMemberInfo(arg.Arg.TargetObj);
                if (info?.Seat != null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                var character = arg.Arg.TargetObj;
                var characterCreationData = new CharacterCreationData(info.CharacterCreationData);

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.PushMenuScreen(nextMenu, arg.Self, arg.User, targetObj: character, other: characterCreationData);
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.Hide(manager, back);
            }
        }
    }
}
