using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class WanderingOpen : ReferableScript, IOpenEffect
    {
        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            Effect.SetTo(self);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            Effect.RemoveFrom(self);
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        private class Effect : IRogueObjUpdater
        {
            private static readonly Effect instance = new Effect();

            float IRogueObjUpdater.Order => 1f;

            public static void SetTo(RogueObj obj)
            {
                if (RogueWalkerInfo.Get(obj) == null) { RogueWalkerInfo.SetTo(obj, new WanderingWalker(RoguegardSettings.MaxTilemapSize)); }
                RogueEffectUtility.AddFromInfoSet(obj, instance);
            }

            public static void RemoveFrom(RogueObj obj)
            {
                RogueEffectUtility.Remove(obj, instance);
                RogueWalkerInfo.RemoveFrom(obj);
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                RogueObjUpdaterContinueType result;
                if (sectionIndex == 0)
                {
                    result = TickUtility.Section0Update(self, ref sectionIndex);
                }
                else
                {
                    Update(activationDepth);
                    result = TickUtility.SectionAfter1LateUpdate(self, ref sectionIndex);
                }
                return result;

                void Update(float activationDepth)
                {
                    if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

                    // 敵がプレイヤーを壁越しに察知して近づいてしまわないように視界距離は固定
                    var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
                    var random = RogueRandom.Primary;

                    // 恐怖状態のとき逃げる
                    if (AutoAction.GetAway(this, self, activationDepth, visibleRadius, room)) return;

                    // スキル・アイテム使用
                    if (AutoAction.TryOtherAction(self, activationDepth, visibleRadius, room, random)) return;

                    // 通常攻撃
                    var attackSkill = AttackUtility.GetNormalAttackSkill(self);
                    if (AutoAction.AutoSkill(MainInfoKw.Attack, attackSkill, self, self, activationDepth, null, visibleRadius, room, random)) return;

                    // 射撃
                    var throwSkill = self.Main.InfoSet.Attack;
                    if (AutoAction.AutoSkill(MainInfoKw.Throw, throwSkill, self, self, activationDepth, null, visibleRadius, room, random)) return;

                    // 移動
                    var walker = RogueWalkerInfo.Get(self);
                    var targetPosition = walker.GetWalk(self, false);
                    if (MovementUtility.TryGetApproachDirection(self, targetPosition, true, out var approachDirection))
                    {
                        this.Walk(self, approachDirection, activationDepth);
                        walker.GetWalk(self, false); // 移動した直後の視界でパスを更新
                    }
                }
            }
        }
    }
}
