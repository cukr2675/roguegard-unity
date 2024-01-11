using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    /// <summary>
    /// <see cref="LobbyMemberInfo.Seat"/> 設定時の所有アイテムの状態を記憶するクラス
    /// </summary>
    [ObjectFormer.Formable]
    public class RogueObjRegister
    {
        private readonly List<Item> items = new List<Item>();

        public int Count => items.Count;

        /// <summary>
        /// <see cref="self"/> の現在の所持アイテムの中から登録された所有アイテムを探す。同時に所有アイテムの装備状態と生成方法を取得する。
        /// </summary>
        public RogueObj GetItem(RogueObj self, int index, out bool itemIsEquipped, out IReadOnlyStartingItem startingItem)
        {
            var listItem = items[index];
            itemIsEquipped = listItem.IsEquipped;
            startingItem = listItem.StartingItem;

            var spaceObjs = self.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null || obj.Main.Stats != listItem.MainStats) continue;

                return obj;
            }
            return null;
        }

        public bool Contains(RogueObj item)
        {
            foreach (var listItem in items)
            {
                if (listItem.MainStats == item.Main.Stats) return true;
            }
            return false;
        }

        /// <summary>
        /// 指定のアイテムを所有アイテムとして登録する
        /// </summary>
        public void Add(RogueObj item)
        {
            var newItem = new Item();
            newItem.MainStats = item.Main.Stats;

            var startingItem = new StartingItemBuilder();
            if (item.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet && itemInfoSet.Data is IStartingItemOption option)
            {
                startingItem.Option = option;
            }
            startingItem.Stack = 1;
            newItem.StartingItem = startingItem;

            var equipmentInfo = item.Main.GetEquipmentInfo(item);
            if (equipmentInfo != null)
            {
                newItem.IsEquipped = equipmentInfo.EquipIndex != -1;
            }

            items.Add(newItem);
        }

        /// <summary>
        /// 指定のインデックスの所有アイテム情報を上書きする
        /// </summary>
        public void SetItem(int index, RogueObj item)
        {
            items[index].MainStats = item.Main.Stats;
        }

        public void Clear()
        {
            items.Clear();
        }

        public void ReplaceCloned(RogueObj obj, RogueObj clonedObj)
        {
            foreach (var listItem in items)
            {
                if (listItem.MainStats == obj.Main.Stats)
                {
                    listItem.MainStats = clonedObj.Main.Stats;
                    return;
                }
            }
        }

        [ObjectFormer.Formable]
        private class Item
        {
            public MainStats MainStats { get; set; }
            public bool IsEquipped { get; set; }
            public StartingItemBuilder StartingItem { get; set; }
        }
    }
}
