using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class AppearanceBuilderList //: IReadOnlyList<AppearanceBuilder>
    {
        private readonly List<AppearanceBuilder> builders = new List<AppearanceBuilder>();

        public AppearanceBuilder this[int index] => builders[index];

        public int Count => builders.Count;

        public void Add(AppearanceBuilder builder)
        {
            builders.Add(builder);
        }

        public void AddRange(IEnumerable<AppearanceBuilder> builders)
        {
            this.builders.AddRange(builders);
        }

        public void Clear()
        {
            builders.Clear();
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
