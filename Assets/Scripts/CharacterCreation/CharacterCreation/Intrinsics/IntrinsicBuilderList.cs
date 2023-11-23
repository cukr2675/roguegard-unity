using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class IntrinsicBuilderList : IEnumerable<IntrinsicBuilder>//, IReadOnlyList<IntrinsicBuilder>
    {
        private readonly List<IntrinsicBuilder> builders = new List<IntrinsicBuilder>();

        public IntrinsicBuilder this[int index] => builders[index];

        public int Count => builders.Count;

        public void Add(IntrinsicBuilder builder)
        {
            builders.Add(builder);
        }

        public void AddRange(IEnumerable<IntrinsicBuilder> builders)
        {
            this.builders.AddRange(builders);
        }

        public void Clear()
        {
            builders.Clear();
        }

        public IEnumerator<IntrinsicBuilder> GetEnumerator() => builders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => builders.GetEnumerator();
    }
}
