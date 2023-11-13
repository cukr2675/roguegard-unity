using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    public class StartingItemBuilderTable //: IReadOnlyList<StartingItemBuilderList>
    {
        private readonly List<StartingItemBuilderList> table = new List<StartingItemBuilderList>();

        public StartingItemBuilderList this[int index] => table[index];

        public int Count => table.Count;

        public void SetValues(IEnumerable<IEnumerable<ScriptableStartingItem>> startingItemTable)
        {
            table.Clear();
            foreach (var startingItemList in startingItemTable)
            {
                var list = new StartingItemBuilderList();
                list.AddRange(startingItemList.Select(x => x.ToBuilder()));
                table.Add(list);
            }
        }

        private IEnumerator<StartingItemBuilderList> GetEnumerator() => table.GetEnumerator();
        public static implicit operator Spanning<IWeightedRogueObjGeneratorList>(StartingItemBuilderTable table)
            => Spanning<IWeightedRogueObjGeneratorList>.Create(table.table);
    }
}
