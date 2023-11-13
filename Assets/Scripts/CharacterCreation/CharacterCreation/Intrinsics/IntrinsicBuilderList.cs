using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    public class IntrinsicBuilderList : IEnumerable<IntrinsicBuilder>//, IReadOnlyList<IntrinsicBuilder>
    {
        private readonly List<IntrinsicBuilder> builders = new List<IntrinsicBuilder>();

        public IntrinsicBuilder this[int index] => builders[index];

        public int Count => builders.Count;

        public void SetValues(IEnumerable<ScriptableIntrinsic> intrinsics)
        {
            builders.Clear();
            builders.AddRange(intrinsics.Select(x => x.ToBuilder()));
        }

        public IEnumerator<IntrinsicBuilder> GetEnumerator() => builders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => builders.GetEnumerator();
    }
}
