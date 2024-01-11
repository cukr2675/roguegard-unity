using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public class SeatBeApplied : BaseApplyRogueMethod
    {
        private static readonly Menu menu = new Menu();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var lobby = RogueWorld.GetLobbyByCharacter(self);
            if (self.Location == lobby)
            {
                var lobbyMembers = LobbyMembers.GetMembersByCharacter(self);
                for (int i = 0; i < lobbyMembers.Count; i++)
                {
                    var member = lobbyMembers[i];
                    if (member == null) continue;

                    var memberInfo = LobbyMembers.GetMemberInfo(member);
                    if (memberInfo.Seat == self)
                    {
                        // 誰か座っていたらそのキャラに注目する
                        RogueDevice.Add(DeviceKw.StartAutoPlay, member);
                        return false;
                    }
                }

                // 誰も座っていなかったら座らせるキャラを選択させる
                RogueDevice.Primary.AddMenu(menu, user, null, new(targetPosition: self.Position));
                return false;
            }

            return false;
        }

        private class Menu : IModelsMenu, IModelsMenuItemController
        {
            public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                var lobbyMembers = LobbyMembers.GetMembersByCharacter(player);
                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, lobbyMembers, root, player, null, arg);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var obj = (RogueObj)model;
                if (obj.Location == null)
                {
                    return obj.GetName();
                }
                else
                {
                    return "<#808080>" + obj.GetName();
                }
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var obj = (RogueObj)model;
                if (obj.Location == null && arg.TryGetTargetPosition(out var position))
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.Done();

                    var info = LobbyMembers.GetMemberInfo(obj);
                    info.Seat = self;

                    info.ItemRegister.Clear();
                    var spaceObjs = obj.Space.Objs;
                    for (int i = 0; i < spaceObjs.Count; i++)
                    {
                        var item = spaceObjs[i];
                        if (item == null) continue;

                        info.ItemRegister.Add(item);
                    }

                    var world = RogueWorld.GetWorld(self);
                    SpaceUtility.TryLocate(obj, world);
                    info.SavePoint = RogueWorld.SavePointInfo;
                    var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                    obj.Main.Stats.TryAssignParty(obj, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                }
                else
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                }
            }
        }
    }
}
