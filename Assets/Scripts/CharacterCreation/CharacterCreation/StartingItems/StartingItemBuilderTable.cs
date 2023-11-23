using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class StartingItemBuilderTable //: IReadOnlyList<StartingItemBuilderList>
    {
        private readonly List<StartingItemBuilderList> table = new List<StartingItemBuilderList>();

        public StartingItemBuilderList this[int index] => table[index];

        public int Count => table.Count;

        public void Add(IEnumerable<StartingItemBuilder> builders)
        {
            table.Add(new StartingItemBuilderList(builders));
        }

        public void AddRange(IEnumerable<IEnumerable<StartingItemBuilder>> builderTable)
        {
            foreach (var builderList in builderTable)
            {
                Add(builderList);
            }
        }

        public void Clear()
        {
            table.Clear();
        }

        private IEnumerator<StartingItemBuilderList> GetEnumerator() => table.GetEnumerator();
        public static implicit operator Spanning<IWeightedRogueObjGeneratorList>(StartingItemBuilderTable table)
            => Spanning<IWeightedRogueObjGeneratorList>.Create(table.table);
    }
}
