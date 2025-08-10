using System.Collections;
using System.Collections.Generic;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable(Objforming.FormerMode.Wrapper)]
    public sealed class StartingItemTable : IEnumerable<StartingItemList>
    {
        private readonly List<StartingItemList> table = new();

        public StartingItemList this[int index] => table[index];

        public int Count => table.Count;

        public Spanning<IWeightedRogueObjGeneratorList> Span => Spanning.Get<IWeightedRogueObjGeneratorList>(table);

        public StartingItemList Add()
        {
            var list = new StartingItemList();
            table.Add(list);
            return list;
        }

        public void AddClones(IEnumerable<IEnumerable<IReadOnlyStartingItem>> startingItemTable)
        {
            foreach (var startingItemList in startingItemTable)
            {
                var list = new StartingItemList();
                list.AddClones(startingItemList);
                table.Add(list);
            }
        }

        public bool Remove(StartingItem startingItem, bool removeEmptyList)
        {
            var any = false;
            for (int i = 0; i < table.Count; i++)
            {
                any |= table[i].Remove(startingItem);
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

        public IEnumerator<StartingItemList> GetEnumerator() => table.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => table.GetEnumerator();
    }
}
