using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using SDSSprite;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class AppearanceBuilderList : IEnumerable<AppearanceBuilder>
    {
        private readonly List<AppearanceBuilder> builders = new List<AppearanceBuilder>();

        public AppearanceBuilder this[int index] => builders[index];

        public int Count => builders.Count;

        public bool TryGetBuilder(BoneKeyword boneName, out AppearanceBuilder builder)
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

        public AppearanceBuilder Add()
        {
            var builder = new AppearanceBuilder();
            builders.Add(builder);
            return builder;
        }

        public void AddClones(IEnumerable<IReadOnlyAppearance> appearances)
        {
            builders.AddRange(appearances.Select(x => new AppearanceBuilder(x)));
        }

        public bool Remove(AppearanceBuilder builder)
        {
            return builders.Remove(builder);
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

        public IEnumerator<AppearanceBuilder> GetEnumerator() => builders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => builders.GetEnumerator();
        public static implicit operator Spanning<IReadOnlyAppearance>(AppearanceBuilderList list)
            => Spanning<IReadOnlyAppearance>.Create(list.builders);
    }
}
