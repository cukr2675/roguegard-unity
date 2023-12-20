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

                    var movement = MovementCalculator.Get(obj);
                    obj.TryLocate(self.Location, position, movement.AsTile, movement.HasCollider, false, movement.HasSightCollider, StackOption.Default);
                    var effect = new Effect();
                    obj.Main.RogueEffects.AddOpen(obj, effect);
                    var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                    obj.Main.Stats.TryAssignParty(obj, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                }
                else if (obj.Main.RogueEffects.TryGetEffect<Effect>(out _))
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.Done();

                    RogueDevice.Add(DeviceKw.StartAutoPlay, obj);
                }
                else
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                }
            }
        }

        [ObjectFormer.Formable]
        private class Effect : IRogueEffect, IRogueObjUpdater, IValueEffect
        {
            [System.NonSerialized] private RogueObj stairs;
            [System.NonSerialized] private IPathBuilder pathBuilder = new AStarPathBuilder(RoguegardSettings.MaxTilemapSize);

            private static CommandFloorDownStairs apply = new CommandFloorDownStairs();

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
                            break;
                        }
                    }
                }
                else
                {
                    if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

                    // 敵がプレイヤーを壁越しに察知して近づいてしまわないように視界距離は固定
                    var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
                    var random = RogueRandom.Primary;

                    // ダンジョンでは階段を目指す
                    var spaceObjs = self.Location.Space.Objs;
                    for (int i = 0; i < spaceObjs.Count; i++)
                    {
                        var obj = spaceObjs[i];
                        if (obj == null) continue;

                        if (obj.Main.InfoSet.Category == CategoryKw.LevelDownStairs)
                        {
                            if (stairs == null || stairs.Location != self.Location)
                            {
                                stairs = obj;
                                pathBuilder.UpdatePath(self, stairs.Position);
                            }

                            var result = pathBuilder.TryGetNextPosition(self, out var nextPosition);
                            if (result &&
                                RogueDirection.TryFromSign(nextPosition - self.Position, out var direction) &&
                                this.Walk(self, direction, activationDepth))
                            {
                                return default;
                            }
                            else if (!result && apply.CommandInvoke(self, null, activationDepth, new(tool: stairs)))
                            {
                                // 階段についたら使う
                                return default;
                            }
                        }
                        else if ((self.Position - obj.Position).sqrMagnitude <= 2 && StatsEffectedValues.AreVS(self, obj))
                        {
                            // 敵と隣接しているとき、敵を攻撃する
                            var attackSkill = AttackUtility.GetNormalAttackSkill(self);
                            if (AutoAction.AutoSkill(MainInfoKw.Attack, attackSkill, self, self, activationDepth, null, visibleRadius, room, random)) return default;
                        }
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
