using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class IntrinsicList : IEnumerable<Intrinsic>
    {
        private readonly List<Intrinsic> list = new();

        public Intrinsic this[int index] => list[index];

        public int Count => list.Count;

        public Spanning<IReadOnlyIntrinsic> Span => Spanning.Get<IReadOnlyIntrinsic>(list);

        public Intrinsic Add()
        {
            var intrinsic = new Intrinsic();
            list.Add(intrinsic);
            return intrinsic;
        }

        public void AddClones(IEnumerable<IReadOnlyIntrinsic> intrinsics)
        {
            list.AddRange(intrinsics.Select(x => new Intrinsic(x)));
        }

        public bool Remove(Intrinsic intrinsic)
        {
            return list.Remove(intrinsic);
        }

        public void Clear()
        {
            list.Clear();
        }

        public IEnumerator<Intrinsic> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
