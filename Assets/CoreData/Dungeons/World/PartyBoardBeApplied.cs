using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public class PartyBoardBeApplied : ReferableScript, IApplyRogueMethod
    {
        private PartyBoardBeApplied() { }

        IRogueMethodTarget ISkillDescription.Target => null;
        IRogueMethodRange ISkillDescription.Range => null;
        int ISkillDescription.RequiredMP => 0;
        Spanning<IKeyword> ISkillDescription.AmmoCategories => Spanning<IKeyword>.Empty;

        private static readonly RogueMenu rogueMenu = new RogueMenu();

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                RogueDevice.Primary.AddMenu(rogueMenu, user, null, RogueMethodArgument.Identity);
                return true;
            }
            else
            {
                return false;
            }
        }

        int ISkillDescription.GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        public class RogueMenu : IModelsMenu
        {
            private static readonly ItemController itemController = new ItemController();
            private static readonly List<object> models = new List<object>();

            public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                var lobbyMembers = RogueWorld.GetLobbyMembersByCharacter(player);
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

                public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (model == null) return "+ 追加";
                    else return ((RogueObj)model).GetName();
                }

                public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (model == null)
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    }
                    else
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        var openArg = new RogueMethodArgument(targetObj: (RogueObj)model);
                        root.OpenMenuAsDialog(nextMenu, self, user, openArg, RogueMethodArgument.Identity);
                    }
                }
            }

            private class CommandMenu : IModelsMenu
            {
                private static readonly object[] models = new object[]
                {
                    new Change(),
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
                    return "交替";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
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
        }
    }
}
