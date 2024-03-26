using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class FruitfulGraceAbility : AbilityIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect
        {
            float IRogueMethodPassiveAspect.Order => 0f;

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveNext next)
            {
                var generate = keyword == MainInfoKw.Locate && self?.Location?.Space.Tilemap == null;

                var result = next.Invoke(keyword, method, self, user, activationDepth, arg);
                if (!result) return false;

                if (generate && self.Location?.Space.Tilemap != null &&
                    DungeonInfo.TryGet(self.Location, out var dungeonInfo) &&
                    dungeonInfo.TryGetLevel(self.Location.Main.Stats.Lv, out var level) &&
                    level.ItemTable.Count >= 1)
                {
                    // 階層移動に成功したとき 10% でアイテムを追加生成する。
                    var random = RogueRandom.Primary;
                    if (random.Next(0, 10) == 0)
                    {
                        var position = self.Location.Space.GetRandomPositionInRoom(random);
                        WeightedRogueObjGeneratorUtility.CreateObj(level.ItemTable[0], self.Location, position, RogueRandom.Primary);
                    }
                }
                return true;
            }
        }
    }
}
