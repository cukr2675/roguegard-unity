using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class RogueObjRegister
    {
        private readonly List<Item> items = new List<Item>();

        public int Count => items.Count;

        public RogueObj GetItem(int index, RogueObj obj, out bool itemIsEquipped, out IReadOnlyStartingItem startingItem)
        {
            var listItem = items[index];
            itemIsEquipped = listItem.IsEquipped;
            startingItem = listItem.StartingItem;

            var spaceObjs = obj.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var spaceObj = spaceObjs[i];
                if (spaceObj == null || spaceObj.Main != listItem.Main) continue;

                return spaceObj;
            }
            return null;
        }

        public void SetItem(int index, RogueObj item)
        {
            items[index].Main = item.Main;
        }

        public bool Contains(RogueObj item)
        {
            foreach (var listItem in items)
            {
                if (listItem.Main == item.Main) return true;
            }
            return false;
        }

        public void Add(RogueObj item)
        {
            var newItem = new Item();
            newItem.Main = item.Main;

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

        public void Clear()
        {
            items.Clear();
        }

        [ObjectFormer.Formable]
        private class Item
        {
            public MainRogueObjInfo Main { get; set; }
            public bool IsEquipped { get; set; }
            public StartingItemBuilder StartingItem { get; set; }
        }
    }
}
