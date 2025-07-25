using Roguegard.CharacterCreation;
using System.Collections.Generic;

namespace Roguegard
{
    /// <summary>
    /// <see cref="LobbyMemberInfo.Seat"/> 設定時の所有アイテムの状態を記憶するクラス
    /// </summary>
    [Objforming.Formable]
    public class RogueObjRegister
    {
        private readonly List<Possessions> possessions = new();

        public int Count => possessions.Count;

        /// <summary>
        /// <see cref="self"/> の現在の所持アイテムの中から登録された所有アイテムを探す。同時に所有アイテムの装備状態と生成方法を取得する。
        /// </summary>
        public RogueObj GetItem(RogueObj self, int index, out bool itemIsEquipped, out IReadOnlyStartingItem startingItem)
        {
            var possession = possessions[index];
            itemIsEquipped = possession.IsEquipped;
            startingItem = possession.StartingItem;

            foreach (var obj in self.Space.Objs)
            {
                if (obj == null || obj.Main.Stats != possession.MainStats) continue;

                return obj;
            }
            return null;
        }

        public bool Contains(RogueObj item)
        {
            foreach (var possession in possessions)
            {
                if (possession.MainStats == item.Main.Stats) return true;
            }
            return false;
        }

        /// <summary>
        /// 指定のアイテムを所有アイテムとして登録する
        /// </summary>
        public void Add(RogueObj item)
        {
            var newPossession = new Possessions
            {
                MainStats = item.Main.Stats
            };

            var startingItem = new StartingItem();
            if (item.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet && itemInfoSet.Data is IStartingItemOption option)
            {
                startingItem.Option = option;
            }
            startingItem.Stack = 1;
            newPossession.StartingItem = startingItem;

            var equipmentInfo = item.Main.GetEquipmentInfo(item);
            if (equipmentInfo != null)
            {
                newPossession.IsEquipped = equipmentInfo.EquippedSubslot != -1;
            }

            possessions.Add(newPossession);
        }

        /// <summary>
        /// 指定のインデックスの所有アイテム情報を上書きする
        /// </summary>
        public void SetItem(int index, RogueObj item)
        {
            possessions[index].MainStats = item.Main.Stats;
        }

        public void Clear()
        {
            possessions.Clear();
        }

        public void ReplaceObj(RogueObj obj, RogueObj clonedObj)
        {
            foreach (var listItem in possessions)
            {
                if (listItem.MainStats == obj.Main.Stats)
                {
                    listItem.MainStats = clonedObj.Main.Stats;
                    return;
                }
            }
        }

        [Objforming.Formable]
        private class Possessions
        {
            public MainStats MainStats { get; set; }
            public bool IsEquipped { get; set; }
            public StartingItem StartingItem { get; set; }
        }
    }
}
