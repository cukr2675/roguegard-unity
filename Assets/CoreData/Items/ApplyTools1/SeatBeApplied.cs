using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public class SeatBeApplied : BaseApplyRogueMethod
    {
        //private static readonly Menu menu = new Menu();

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
                //RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
                return false;
            }

            return false;
        }

        //private class Menu : IListMenu, IElementPresenter
        //{
        //    public void OpenMenu(IListMenuManager manager, RogueObj player, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var worldInfo = RogueWorldInfo.GetByCharacter(player);
        //        var lobbyMembers = worldInfo.LobbyMembers.Members;
        //        var scroll = manager.GetView(DeviceKw.MenuScroll);
        //        scroll.OpenView(this, lobbyMembers, manager, player, null, arg);
        //        ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
        //    }

        //    public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var obj = (RogueObj)element;
        //        if (obj.Location == null)
        //        {
        //            return obj.GetName();
        //        }
        //        else
        //        {
        //            return "<#808080>" + obj.GetName();
        //        }
        //    }

        //    public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var lobbyMember = (RogueObj)element;
        //        if (lobbyMember.Location == null)
        //        {
        //            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
        //            manager.Done();

        //            var info = LobbyMemberList.GetMemberInfo(lobbyMember);
        //            info.Seat = arg.TargetObj;

        //            info.ItemRegister.Clear();
        //            var spaceObjs = lobbyMember.Space.Objs;
        //            for (int i = 0; i < spaceObjs.Count; i++)
        //            {
        //                var item = spaceObjs[i];
        //                if (item == null) continue;

        //                info.ItemRegister.Add(item);
        //            }

        //            var world = RogueWorldInfo.GetWorld(self);
        //            SpaceUtility.TryLocate(lobbyMember, world);
        //            info.SavePoint = RogueWorldSavePointInfo.Instance;
        //            var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
        //            lobbyMember.Main.Stats.TryAssignParty(lobbyMember, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
        //        }
        //        else
        //        {
        //            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
        //        }
        //    }
        //}
    }
}
