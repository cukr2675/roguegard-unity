using Roguegard.Extensions;
using UnityEngine;

namespace Roguegard
{
    public class StorageBehaviourNode : IRogueBehaviourNode
    {
        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;

            var memberInfo = LobbyMemberList.GetMemberInfo(self);
            if (memberInfo == null) return RogueObjUpdaterContinueType.Continue;

            RogueObj nearestContainer = null;
            int nearestSqrDistance = int.MaxValue;
            foreach (var obj in self.Location.Space.Objs)
            {
                if (obj == null || obj.Main.InfoSet.Category != CategoryKw.Container) continue;

                var sqrDistance = (obj.Position - self.Position).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestContainer = obj;
                    nearestSqrDistance = sqrDistance;
                }
            }
            if (nearestContainer == null) return RogueObjUpdaterContinueType.Continue;

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

            var containerInfo = ContainerInfo.GetInfo(nearestContainer);
            var items = self.Space.Objs;
            for (int i = 0; i < items.Length; i++) // アイテムの移動でオブジェクト数が増加する可能性がある
            {
                var item = items[i];
                if (item == null || itemRegister.Contains(item)) continue;

                // 持たせたアイテム以外を入れ物にしまう
                RogueMethodAspectState.Invoke(MainInfoKw.Walk, containerInfo.TakeIn, nearestContainer, self, activationDepth, new(targetObj: item));
            }
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
