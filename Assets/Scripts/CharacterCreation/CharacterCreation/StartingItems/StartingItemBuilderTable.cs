using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class StartingItemBuilderTable : IEnumerable<StartingItemBuilderList>
    {
        private readonly List<StartingItemBuilderList> table = new List<StartingItemBuilderList>();

        public StartingItemBuilderList this[int index] => table[index];

        public int Count => table.Count;

        public StartingItemBuilderList Add()
        {
            var builders = new StartingItemBuilderList();
            table.Add(builders);
            return builders;
        }

        public void AddClones(IEnumerable<IEnumerable<IReadOnlyStartingItem>> startingItemTable)
        {
            foreach (var startingItemList in startingItemTable)
            {
                var list = new StartingItemBuilderList();
                list.AddClones(startingItemList);
                table.Add(list);
            }
        }

        public void Clear()
        {
            table.Clear();
        }

        public IEnumerator<StartingItemBuilderList> GetEnumerator() => table.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => table.GetEnumerator();
        public static implicit operator Spanning<IWeightedRogueObjGeneratorList>(StartingItemBuilderTable table)
            => Spanning<IWeightedRogueObjGeneratorList>.Create(table.table);
    }
}
