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

        public IntrinsicBuilder Add()
        {
            var builder = new IntrinsicBuilder();
            builders.Add(builder);
            return builder;
        }

        public void AddClones(IEnumerable<IReadOnlyIntrinsic> intrinsics)
        {
            builders.AddRange(intrinsics.Select(x => new IntrinsicBuilder(x)));
        }

        public bool Remove(IntrinsicBuilder builder)
        {
            return builders.Remove(builder);
        }

        public void Clear()
        {
            builders.Clear();
        }

        public IEnumerator<IntrinsicBuilder> GetEnumerator() => builders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => builders.GetEnumerator();
        public static implicit operator Spanning<IReadOnlyIntrinsic>(IntrinsicBuilderList list)
            => Spanning<IReadOnlyIntrinsic>.Create(list.builders);
    }
}
