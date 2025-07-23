using OchalikeSprites;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class AppearanceList : IEnumerable<Appearance>
    {
        private readonly List<Appearance> list = new();

        public Appearance this[int index] => list[index];

        public int Count => list.Count;

        public Spanning<IReadOnlyAppearance> Span => Spanning.Get<IReadOnlyAppearance>(list);

        public bool TryGetValue(BoneKeyword boneName, out Appearance appearance)
        {
            foreach (var listAppearance in list)
            {
                if (listAppearance.Option.BoneName == boneName)
                {
                    appearance = listAppearance;
                    return true;
                }
            }
            appearance = null;
            return false;
        }

        public Appearance Add()
        {
            var appearance = new Appearance();
            list.Add(appearance);
            return appearance;
        }

        public void AddClones(IEnumerable<IReadOnlyAppearance> appearances)
        {
            list.AddRange(appearances.Select(x => new Appearance(x)));
        }

        public bool Remove(Appearance appearance)
        {
            return list.Remove(appearance);
        }

        public void Clear()
        {
            list.Clear();
        }

        public AppearanceList Clone()
        {
            var clone = new AppearanceList();
            clone.list.AddRange(list);
            return clone;
        }

        public IEnumerator<Appearance> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
