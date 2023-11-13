using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class StartingItemBuilderList : IWeightedRogueObjGeneratorList//, IReadOnlyList<StartingItemBuilder>
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

        public void AddRange(IEnumerable<StartingItemBuilder> builders)
        {
            this.builders.AddRange(builders);
        }

        private IEnumerator<StartingItemBuilder> GetEnumerator()
        {
            return builders.GetEnumerator();
        }
    }
}
