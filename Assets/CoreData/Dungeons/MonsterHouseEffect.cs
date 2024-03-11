using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class MonsterHouseEffect : IRogueEffect, IRogueMethodPassiveAspect
    {
        public int RoomIndex { get; private set; }

        float IRogueMethodPassiveAspect.Order => 0f;

        private MonsterHouseEffect() { }

        public static void SetTo(RogueObj location, int roomIndex)
        {
            var effect = new MonsterHouseEffect();
            effect.RoomIndex = roomIndex;
            location.Main.RogueEffects.AddOpen(location, effect);
        }

        public void Open(RogueObj self)
        {
            RogueEffectUtility.AddFromRogueEffect(self, this);
        }

        bool IRogueMethodPassiveAspect.PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveNext next)
        {
            var result = next.Invoke(keyword, method, self, user, activationDepth, arg);
            RogueDungeonLevel level = null;
            if (result && keyword == StdKw.BeEntered && RoomIndex < self.Space.RoomCount)
            {
                self.Space.GetRoom(RoomIndex, out var monsterHouse, out _);
                if (self.Space.TryGetRoomView(user.Position, out var userRoom, out _) && userRoom.Equals(monsterHouse) &&
                    (DungeonInfo.Get(self)?.TryGetLevel(self.Main.Stats.Lv, out level) ?? false) && level.EnemyTable.Count >= 1)
                {
                    if (RogueDevice.Primary.Subject.Main.Stats.Party.Members.Contains(user))
                    {
                        RogueDevice.Add(DeviceKw.AppendText, "モンスターハウスだ！\n");
                    }

                    // 敵を生成
                    var enemies = level.EnemyTable[0];
                    var random = RogueRandom.Primary;
                    for (int i = 0; i < 10; i++)
                    {
                        var position = self.Space.GetRandomPositionInRoom(random, RoomIndex);
                        WeightedRogueObjGeneratorUtility.CreateObj(enemies, self, position, random);
                    }

                    // エフェクトを解除
                    RogueEffectUtility.RemoveClose(self, this);
                }
            }
            return result;
        }

        public bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
        public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
        public IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
    }
}
