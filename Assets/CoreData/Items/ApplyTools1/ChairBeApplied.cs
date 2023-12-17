using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;
using Roguegard.Extensions;

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
                var lobbyMembers = (Spanning<RogueObj>)RogueWorld.GetLobbyMembersByCharacter(player);
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

                    var movement = MovementCalculator.Get(obj);
                    var r = obj.TryLocate(self.Location, position, movement.AsTile, movement.HasCollider, false, movement.HasSightCollider, StackOption.Default);
                    Debug.Log(r);
                }
                else
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                }
            }
        }

        private class Effect : IRogueEffect, IRogueObjUpdater, IValueEffect
        {

            float IRogueObjUpdater.Order => 1f;
            float IValueEffect.Order => 0f;

            public void Open(RogueObj self)
            {
                RogueEffectUtility.AddFromRogueEffect(self, this);
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                var lobby = RogueWorld.GetLobbyByCharacter(self);
                if (self.Location == lobby)
                {
                    // ロビーではダンジョンに移動
                    foreach (var value in RoguegardSettings.GetAssetTable("Core").Values)
                    {
                        if (value is CharacterCreation.DungeonCreationData dungeon)
                        {
                            dungeon.StartDungeon(self, RogueRandom.Primary);
                            dungeon.StartFloor(self, RogueRandom.Primary);
                        }
                    }
                }
                else
                {
                    // ダンジョンでは階段を目指す
                    var spaceObjs = self.Location.Space.Objs;
                    for (int i = 0; i < spaceObjs.Count; i++)
                    {
                        var obj = spaceObjs[i];
                        if (obj == null || obj.Main.InfoSet.Category != CategoryKw.LevelDownStairs) continue;

                        //WanderingWalker
                    }
                }
                return default;
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StdKw.View)
                {
                    value.MainValue += 10000f;
                }
            }

            public bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
            public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
