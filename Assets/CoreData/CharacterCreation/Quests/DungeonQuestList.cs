using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class DungeonQuestList
    {
        private readonly List<DungeonQuest> quests = new List<DungeonQuest>();

        public DungeonQuest this[int index] => quests[index];

        public int Count => quests.Count;

        public void Add(DungeonQuest quest) => quests.Add(quest);
        public void RemoveAt(int index) => quests.RemoveAt(index);
    }
}
