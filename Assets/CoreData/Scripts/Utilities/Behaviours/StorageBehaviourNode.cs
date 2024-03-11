using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class StorageBehaviourNode : IRogueBehaviourNode
    {
        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;

            var memberInfo = LobbyMemberList.GetMemberInfo(self);
            if (memberInfo == null) return RogueObjUpdaterContinueType.Continue;

            var spaceObjs = self.Location.Space.Objs;
            RogueObj nearestChest = null;
            int nearestSqrDistance = int.MaxValue;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null || obj.Main.InfoSet.Category != CategoryKw.Chest) continue;

                var sqrDistance = (obj.Position - self.Position).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestChest = obj;
                    nearestSqrDistance = sqrDistance;
                }
            }
            if (nearestChest == null) return RogueObjUpdaterContinueType.Continue;

            var itemRegister = memberInfo.ItemRegister;
            for (int i = 0; i < itemRegister.Count; i++)
            {
                var item = itemRegister.GetItem(self, i, out var itemIsEquipped, out var startingItem);
                if (item == null)
                {
                    item = startingItem.Option.CreateObj(startingItem, self, Vector2Int.zero, RogueRandom.Primary);
                    itemRegister.SetItem(i, item);
                }

                // 装備品を浄化
                EquipmentUtility.Cleansing(item);

                if (itemIsEquipped && item.Main.GetEquipmentInfo(item).EquipIndex == -1)
                {
                    // 装備品を装備しなおす
                    default(IActiveRogueMethodCaller).TryEquip(item, self, activationDepth);
                }
            }

            var chestInfo = ChestInfo.GetInfo(nearestChest);
            var items = self.Space.Objs;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item == null || itemRegister.Contains(item)) continue;

                // 持たせたアイテム以外をチェストにしまう
                RogueMethodAspectState.Invoke(MainInfoKw.Walk, chestInfo.TakeIn, nearestChest, self, activationDepth, new(targetObj: item));
            }
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
