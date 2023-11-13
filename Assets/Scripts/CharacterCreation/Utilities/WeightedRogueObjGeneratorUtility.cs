using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class WeightedRogueObjGeneratorUtility
    {
        public static RogueObj CreateObj(
            IWeightedRogueObjGeneratorList weightedObjList, RogueObj location, Vector2Int position, IRogueRandom random,
            StackOption stackOption = StackOption.Default)
        {
            var sumWeight = random.NextFloat(0f, weightedObjList.TotalWeight);
            var weightedObjs = weightedObjList.Spanning;
            if (weightedObjs.Count == 0) throw new RogueException($"{weightedObjList} の生成候補オブジェクトが一つも存在しません。");

            for (int i = 0; i < weightedObjs.Count - 1; i++)
            {
                var item = weightedObjs[i];
                sumWeight -= item.Weight;
                if (sumWeight <= 0f) return item.CreateObj(location, position, random, stackOption);
            }
            {
                // 計算誤差を考慮して最後のオブジェクトを生成する。
                var item = weightedObjs[weightedObjs.Count - 1];
                return item.CreateObj(location, position, random, stackOption);
            }
        }

        public static RogueObj CreateObj(
            IWeightedRogueObjGeneratorList weightedObjList, RogueObj location, IRogueRandom random,
            StackOption stackOption = StackOption.Default)
        {
            if (location != null && location.Space.Tilemap != null)
                throw new System.ArgumentException("タイルマップを持つオブジェクトへ移動する場合、位置（Position）が必要です。");

            return CreateObj(weightedObjList, location, Vector2Int.zero, random, stackOption);
        }

        public static void CreateObjs(
            Spanning<IWeightedRogueObjGeneratorList> weightedObjTable, RogueObj location, Vector2Int position, IRogueRandom random,
            StackOption stackOption = StackOption.Default)
        {
            for (int i = 0; i < weightedObjTable.Count; i++)
            {
                var weightedObjList = weightedObjTable[i];
                CreateObj(weightedObjList, location, position, random, stackOption);
            }
        }

        public static void CreateObjs(
            Spanning<IWeightedRogueObjGeneratorList> weightedObjTable, RogueObj location, IRogueRandom random,
            StackOption stackOption = StackOption.Default)
        {
            if (location != null && location.Space.Tilemap != null)
                throw new System.ArgumentException("タイルマップを持つオブジェクトへ移動する場合、位置（Position）が必要です。");

            CreateObjs(weightedObjTable, location, Vector2Int.zero, random, stackOption);
        }
    }
}
