using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class IntrinsicList : IEnumerable<Intrinsic>
    {
        private readonly List<Intrinsic> builders = new();

        public Intrinsic this[int index] => builders[index];

        public int Count => builders.Count;

        public Spanning<IReadOnlyIntrinsic> Span => Spanning.Get<IReadOnlyIntrinsic>(builders);

        public Intrinsic Add()
        {
            var builder = new Intrinsic();
            builders.Add(builder);
            return builder;
        }

        public void AddClones(IEnumerable<IReadOnlyIntrinsic> intrinsics)
        {
            builders.AddRange(intrinsics.Select(x => new Intrinsic(x)));
        }

        public bool Remove(Intrinsic builder)
        {
            return builders.Remove(builder);
        }

        public void Clear()
        {
            builders.Clear();
        }

        public IEnumerator<Intrinsic> GetEnumerator() => builders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => builders.GetEnumerator();
    }
}
