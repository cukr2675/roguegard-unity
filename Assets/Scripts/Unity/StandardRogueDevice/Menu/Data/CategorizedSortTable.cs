using Roguegard;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public class CategorizedSortTable
    {
        private readonly Dictionary<IKeyword, RogueObjList[]> categorizedBufferTable;

        private static readonly OtherKeyword other = new();
        private static readonly Comparer comparer = new();
        private static readonly RogueObjList mainBuffer = new();

        public CategorizedSortTable(Spanning<IKeyword> categories)
        {
            categorizedBufferTable = new Dictionary<IKeyword, RogueObjList[]>();
            foreach (var category in categories)
            {
                categorizedBufferTable.Add(category, new[] { new RogueObjList(), new RogueObjList(), new RogueObjList(), new RogueObjList() });
            }
            categorizedBufferTable.Add(other, new[] { new RogueObjList(), new RogueObjList(), new RogueObjList(), new RogueObjList() });
        }

        public void Sort(RogueObj location)
        {
            // テーブルを初期化
            foreach (var pair in categorizedBufferTable)
            {
                foreach (var listItem in pair.Value)
                {
                    listItem.Clear();
                }
            }

            // アイテムを並べ替える
            var objs = location.Space.Objs;
            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                if (obj == null) continue;

                // オブジェクトのカテゴリごとのリストに振り分ける
                if (obj.Main.InfoSet.Category == null || !categorizedBufferTable.TryGetValue(obj.Main.InfoSet.Category, out var categorizedBuffers))
                {
                    categorizedBuffers = categorizedBufferTable[other];
                }
                var weight = WeightCalculator.Get(obj);
                var equipmentInfo = obj.Main.GetEquipmentInfo(obj);
                var vehicleInfo = VehicleInfo.Get(obj);
                var equipped = equipmentInfo?.EquippedSubslot >= 0 || vehicleInfo?.Rider != null;

                // 重さゼロのアイテムを上に並べる
                if (weight.TotalWeight <= 0f)
                {
                    // その中でも装備済みのアイテムを最も上に並べる
                    if (equipped) { categorizedBuffers[0].Add(obj); }
                    else { categorizedBuffers[1].Add(obj); }
                }
                else
                {
                    // 装備済みのアイテムを上に並べる
                    if (equipped) { categorizedBuffers[2].Add(obj); }
                    else { categorizedBuffers[3].Add(obj); }
                }
            }

            // バッファをもとに並べ替える
            mainBuffer.Clear();
            foreach (var pair in categorizedBufferTable)
            {
                foreach (var listItem in pair.Value)
                {
                    // 同じクラスのアイテムは名前順で並べる
                    listItem.Sort(comparer);

                    for (int i = 0; i < listItem.Count; i++)
                    {
                        mainBuffer.Add(listItem[i]);
                    }
                }
            }
            location.Space.Sort(mainBuffer.Span);
        }

        private class OtherKeyword : IKeyword
        {
            public string Name => null;
            public Sprite Icon => null;
            public Color Color => default;
            public string Caption => null;
            public IRogueDetails Details => null;
            public Spanning<IKeyword> Tags => Spanning<IKeyword>.Empty;
        }

        private class Comparer : IComparer<RogueObj>
        {
            public int Compare(RogueObj x, RogueObj y)
            {
                if (x == null || y == null) return 0;

                return string.CompareOrdinal(x.Main.InfoSet.Name, y.Main.InfoSet.Name);
            }
        }
    }
}
