using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    public class AppearanceBuilderList //: IReadOnlyList<AppearanceBuilder>
    {
        private readonly List<AppearanceBuilder> builders = new List<AppearanceBuilder>();

        public AppearanceBuilder this[int index] => builders[index];

        public int Count => builders.Count;

        public void SetValues(IEnumerable<ScriptableAppearance> appearances)
        {
            builders.Clear();
            builders.AddRange(appearances.Select(x => x.ToBuilder()));
        }

        public AppearanceBuilderList Clone()
        {
            var clone = new AppearanceBuilderList();
            clone.builders.AddRange(builders);
            return clone;
        }

        private IEnumerator<AppearanceBuilder> GetEnumerator() => builders.GetEnumerator();
        public static implicit operator Spanning<IReadOnlyAppearance>(AppearanceBuilderList list)
            => Spanning<IReadOnlyAppearance>.Create(list.builders);
    }
}
