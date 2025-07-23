using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class StartingItemList : IWeightedRogueObjGeneratorList, IEnumerable<StartingItem>
    {
        private readonly List<StartingItem> list = new();

        public StartingItem this[int index] => list[index];

        public int Count => list.Count;

        public float TotalWeight
        {
            get
            {
                var totalWeight = 0f;
                foreach (var listStartingItem in list)
                {
                    totalWeight += listStartingItem.GeneratorWeight;
                }
                return totalWeight;
            }
        }

        public Spanning<IWeightedRogueObjGenerator> Span => Spanning.Get<IWeightedRogueObjGenerator>(list);

        int IWeightedRogueObjGeneratorList.MinFrequency => 1;
        int IWeightedRogueObjGeneratorList.MaxFrequency => 1;

        public StartingItem Add()
        {
            var startingItem = new StartingItem();
            list.Add(startingItem);
            return startingItem;
        }

        public void AddClones(IEnumerable<IReadOnlyStartingItem> startingItems)
        {
            list.AddRange(startingItems.Select(x => new StartingItem(x)));
        }

        public bool Remove(StartingItem startingItem)
        {
            return list.Remove(startingItem);
        }

        public IEnumerator<StartingItem> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
