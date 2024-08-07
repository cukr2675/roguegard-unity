using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class StartingItemBuilderList : IWeightedRogueObjGeneratorList, IEnumerable<StartingItemBuilder>
    {
        private readonly List<StartingItemBuilder> builders = new List<StartingItemBuilder>();

        public StartingItemBuilder this[int index] => builders[index];

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

        public Spanning<IWeightedRogueObjGenerator> Spanning => Spanning<IWeightedRogueObjGenerator>.Create(builders);

        int IWeightedRogueObjGeneratorList.MinFrequency => 1;
        int IWeightedRogueObjGeneratorList.MaxFrequency => 1;

        public StartingItemBuilder Add()
        {
            var builder = new StartingItemBuilder();
            builders.Add(builder);
            return builder;
        }

        public void AddClones(IEnumerable<IReadOnlyStartingItem> startingItems)
        {
            builders.AddRange(startingItems.Select(x => new StartingItemBuilder(x)));
        }

        public bool Remove(StartingItemBuilder builder)
        {
            return builders.Remove(builder);
        }

        public IEnumerator<StartingItemBuilder> GetEnumerator() => builders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => builders.GetEnumerator();
    }
}
