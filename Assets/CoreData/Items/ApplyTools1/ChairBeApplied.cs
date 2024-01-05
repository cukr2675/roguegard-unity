using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class ChairBeApplied : BaseApplyRogueMethod
    {
        private static readonly Menu menu = new Menu();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var lobby = RogueWorld.GetLobbyByCharacter(self);
            if (self.Location == lobby)
            {
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

                    var ifInLobby = new IfInLobbyBehaviourNode();
                    ifInLobby.InLobbyNode.Add(new StorageBehaviourNode());
                    ifInLobby.InLobbyNode.Add(new AcceptQuestBehaviourNode());
                    ifInLobby.OtherNode.Add(new AttackBehaviourNode());
                    ifInLobby.OtherNode.Add(new PushObstacleBehaviourNode());

                    var explore = new ExploreForStairsBehaviourNode();
                    explore.PathBuilder = new AStarPathBuilder(RoguegardSettings.MaxTilemapSize);
                    explore.PositionSelector = new RoguePositionSelector();
                    ifInLobby.OtherNode.Add(explore);

                    obj.SetInfo(new ViewInfo());

                    var node = new RogueBehaviourNodeList();
                    node.Add(ifInLobby);
                    var info = LobbyMembers.GetMemberInfo(obj);
                    info.SetSeat(obj, self, node);

                    var world = RogueWorld.GetWorld(self);
                    SpaceUtility.TryLocate(obj, world);
                    info.SavePoint = RogueWorld.SavePointInfo;
                    //var movement = MovementCalculator.Get(obj);
                    //obj.TryLocate(self.Location, position, movement.AsTile, movement.HasCollider, false, movement.HasSightCollider, StackOption.Default);
                    //var effect = new Effect();
                    //effect.chairPosition = position;
                    //obj.Main.RogueEffects.AddOpen(obj, effect);
                    var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                    obj.Main.Stats.TryAssignParty(obj, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                }
                else// if (obj.Main.RogueEffects.TryGetEffect<Effect>(out _))
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.Done();

                    RogueDevice.Add(DeviceKw.StartAutoPlay, obj);
                }
                //else
                //{
                //    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                //}
            }
        }
    }
}
