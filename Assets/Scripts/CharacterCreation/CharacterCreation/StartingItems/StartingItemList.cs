using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class StartingItemList : IWeightedRogueObjGeneratorList, IEnumerable<StartingItem>
    {
        private readonly List<StartingItem> builders = new();

        public StartingItem this[int index] => builders[index];

        public int Count => builders.Count;

        public float TotalWeight
        {
            get
            {
                var totalWeight = 0f;
                foreach (var builder in builders)
                {
                    totalWeight += builder.GeneratorWeight;
                }
                return totalWeight;
            }
        }

        public Spanning<IWeightedRogueObjGenerator> Span => Spanning.Get<IWeightedRogueObjGenerator>(builders);

        int IWeightedRogueObjGeneratorList.MinFrequency => 1;
        int IWeightedRogueObjGeneratorList.MaxFrequency => 1;

        public StartingItem Add()
        {
            var builder = new StartingItem();
            builders.Add(builder);
            return builder;
        }

        public void AddClones(IEnumerable<IReadOnlyStartingItem> startingItems)
        {
            builders.AddRange(startingItems.Select(x => new StartingItem(x)));
        }

        public bool Remove(StartingItem builder)
        {
            return builders.Remove(builder);
        }

        public IEnumerator<StartingItem> GetEnumerator() => builders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => builders.GetEnumerator();
    }
}
