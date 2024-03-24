using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class DungeonQuestTable
    {
        private readonly List<Item> items = new List<Item>();

        public int Count => items.Count;

        public void GetItem(int index, out DungeonQuest quest, out RogueParty party, out int weightTurns)
        {
            var item = items[index];

            // パーティメンバーがいないならそのパーティは削除する
            if (item.party != null && item.party.Members.Count == 0)
            {
                item.party = null;
                item.weightTurns = 0;
            }

            quest = item.quest;
            party = item.party;
            weightTurns = item.weightTurns;
        }

        public void Add(DungeonQuest quest) => items.Add(new Item() { quest = quest });
        public void RemoveAt(int index) => items.RemoveAt(index);

        public bool TryAcceptAt(int index, RogueParty party, int weightTurns)
        {
            var item = items[index];
            if (item.party != null) return false; // 他のパーティが受注済み
            if (Contains(party)) return false; // パーティがすでに他のクエストを受注している

            item.party = party;
            item.weightTurns = weightTurns;
            return true;
        }

        public void Cancel(RogueParty party)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.party != party) continue;

                items.RemoveAt(i);
                return;
            }
        }

        public bool Contains(RogueParty party)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].party == party) return true;
            }
            return false;
        }

        public void UpdateWeightTurns(int deltaTurns = -1)
        {
            foreach (var item in items)
            {
                if (item.weightTurns >= 1) { item.weightTurns += deltaTurns; }
            }
        }

        [ObjectFormer.Formable]
        private class Item
        {
            public DungeonQuest quest;
            public RogueParty party;
            public int weightTurns;
        }
    }
}
