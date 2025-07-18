using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class StartingItemBuilderTable : IEnumerable<StartingItemBuilderList>
    {
        private readonly List<StartingItemBuilderList> table = new();

        public StartingItemBuilderList this[int index] => table[index];

        public int Count => table.Count;

        public Spanning<IWeightedRogueObjGeneratorList> Span => Spanning.Get<IWeightedRogueObjGeneratorList>(table);

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

        public bool Remove(StartingItemBuilder builder, bool removeEmptyList)
        {
            var any = false;
            for (int i = 0; i < table.Count; i++)
            {
                any |= table[i].Remove(builder);
                if (removeEmptyList && table[i].Count == 0)
                {
                    table.RemoveAt(i);
                    i--;
                }
            }
            return any;
        }

        public void Clear()
        {
            table.Clear();
        }

        public IEnumerator<StartingItemBuilderList> GetEnumerator() => table.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => table.GetEnumerator();
    }
}
