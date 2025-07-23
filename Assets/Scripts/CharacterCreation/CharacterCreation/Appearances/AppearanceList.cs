using OchalikeSprites;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class AppearanceList : IEnumerable<Appearance>
    {
        private readonly List<Appearance> builders = new();

        public Appearance this[int index] => builders[index];

        public int Count => builders.Count;

        public Spanning<IReadOnlyAppearance> Span => Spanning.Get<IReadOnlyAppearance>(builders);

        public bool TryGetBuilder(BoneKeyword boneName, out Appearance builder)
        {
            foreach (var item in builders)
            {
                if (item.Option.BoneName == boneName)
                {
                    builder = item;
                    return true;
                }
            }
            builder = null;
            return false;
        }

        public Appearance Add()
        {
            var builder = new Appearance();
            builders.Add(builder);
            return builder;
        }

        public void AddClones(IEnumerable<IReadOnlyAppearance> appearances)
        {
            builders.AddRange(appearances.Select(x => new Appearance(x)));
        }

        public bool Remove(Appearance builder)
        {
            return builders.Remove(builder);
        }

        public void Clear()
        {
            builders.Clear();
        }

        public AppearanceList Clone()
        {
            var clone = new AppearanceList();
            clone.builders.AddRange(builders);
            return clone;
        }

        public IEnumerator<Appearance> GetEnumerator() => builders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => builders.GetEnumerator();
    }
}
