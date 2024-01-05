using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class StorageBehaviourNode : IRogueBehaviourNode
    {
        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;

            var memberInfo = LobbyMembers.GetMemberInfo(self);
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

            var chestInfo = ChestInfo.GetInfo(nearestChest);
            var items = self.Space.Objs;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item == null || Contains(item, memberInfo.CharacterCreationData.StartingItemTable)) continue;

                // チェストに初期アイテム以外をしまう
                RogueMethodAspectState.Invoke(MainInfoKw.Walk, chestInfo.TakeIn, nearestChest, self, activationDepth, new(targetObj: item));
            }
            return RogueObjUpdaterContinueType.Continue;
        }

        private static bool Contains(RogueObj item, Spanning<IWeightedRogueObjGeneratorList> startingItems)
        {
            for (int i = 0; i < startingItems.Count; i++)
            {
                var list = startingItems[i].Spanning;
                for (int j = 0; j < list.Count; j++)
                {
                    if (item.Main.InfoSet == list[j].InfoSet) return true;
                }
            }
            return false;
        }
    }
}
