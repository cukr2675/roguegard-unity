using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.Device;

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
                var lobbyMembers = worldInfo.LobbyMembers.Members;
                for (int i = 0; i < lobbyMembers.Count; i++)
                {
                    var member = lobbyMembers[i];
                    if (member == null) continue;

                    var memberInfo = LobbyMemberList.GetMemberInfo(member);
                    if (memberInfo.Seat == self)
                    {
                        // 誰か座っていたらそのキャラに注目する
                        RogueDevice.Add(DeviceKw.StartAutoPlay, member);
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

            private readonly ScrollViewTemplate<RogueObj, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var player = arg.Self;
                var worldInfo = RogueWorldInfo.GetByCharacter(player);
                var lobbyMembers = worldInfo.LobbyMembers.Members;
                objs.Clear();
                for (int i = 0; i < lobbyMembers.Count; i++)
                {
                    objs.Add(lobbyMembers[i]);
                }

                view.ShowTemplate(objs, manager, arg)
                    ?
                    .ElementNameFrom((lobbyMember, manager, arg) =>
                    {
                        if (lobbyMember.Location == null)
                        {
                            return lobbyMember.GetName();
                        }
                        else
                        {
                            return "<#808080>" + lobbyMember.GetName();
                        }
                    })

                    .OnClickElement((lobbyMember, manager, arg) =>
                    {
                        if (lobbyMember.Location == null)
                        {
                            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                            manager.Done();

                            var info = LobbyMemberList.GetMemberInfo(lobbyMember);
                            info.Seat = arg.Arg.TargetObj;

                            info.ItemRegister.Clear();
                            var spaceObjs = lobbyMember.Space.Objs;
                            for (int i = 0; i < spaceObjs.Count; i++)
                            {
                                var item = spaceObjs[i];
                                if (item == null) continue;

                                info.ItemRegister.Add(item);
                            }

                            var world = RogueWorldInfo.GetWorld(arg.Self);
                            SpaceUtility.TryLocate(lobbyMember, world);
                            info.SavePoint = RogueWorldSavePointInfo.Instance;
                            var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                            lobbyMember.Main.Stats.TryAssignParty(lobbyMember, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                        }
                        else
                        {
                            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                        }
                    })

                    .Build();
            }
        }
    }
}
