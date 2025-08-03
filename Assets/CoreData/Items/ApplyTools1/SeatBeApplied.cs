using Lysionium;
using Lysionium.MergeExtensions.R3;
using Roguegard.Device;
using System.Collections.Generic;

namespace Roguegard
{
    public class SeatBeApplied : BaseApplyRogueMethod
    {
        private static readonly MenuScreen menu = new();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(self);
            if (self.Location == worldInfo.Lobby)
            {
                foreach (var lobbyMember in worldInfo.LobbyMembers.Members)
                {
                    if (lobbyMember == null) continue;

                    var memberInfo = LobbyMemberList.GetMemberInfo(lobbyMember);
                    if (memberInfo.Seat == self)
                    {
                        // 誰か座っていたらそのキャラに注目する
                        RogueDevice.Add(DeviceKw.StartAutoPlay, lobbyMember);
                        return false;
                    }
                }

                // 誰も座っていなかったら座らせるキャラを選択させる
                RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
                return false;
            }

            return false;
        }

        private class MenuScreen : RogueMenuScreen
        {
            private readonly List<RogueObj> objs = new();

            private readonly ScrollViewData<RogueObj, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var player = arg.Self;
                var worldInfo = RogueWorldInfo.GetByCharacter(player);
                objs.Clear();
                foreach (var lobbyMember in worldInfo.LobbyMembers.Members)
                {
                    objs.Add(lobbyMember);
                }

                view.Show(objs, manager, arg)
                    ?
                    .Merge(out var merged)
                    .Init(
                        () => merged

                        .NameFrom(lobbyMember => lobbyMember.GetName())

                        .Case(
                            lo => lo.Location == null,
                            _ => _
                            .OnClick((lobbyMember, manager, arg) =>
                            {
                                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                                manager.Done();
                                
                                var info = LobbyMemberList.GetMemberInfo(lobbyMember);
                                info.Seat = arg.Arg.TargetObj;
                                
                                info.ItemRegister.Clear();
                                foreach (var item in lobbyMember.Space.Objs)
                                {
                                    if (item == null) continue;

                                    info.ItemRegister.Add(item);
                                }

                                var world = RogueWorldInfo.GetWorld(arg.Self);
                                SpaceUtility.TryLocate(lobbyMember, world);
                                info.SavePoint = RogueWorldSavePointInfo.Instance;
                                var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                                lobbyMember.Main.Stats.TryAssignParty(lobbyMember, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                            }))

                        .Otherwise(
                            _ => _
                            .StyleFrom("Disabled")))

                    .Build();
            }
        }
    }
}
