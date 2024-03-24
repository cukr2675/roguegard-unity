using System;
using System.Collections;
using System.Collections.Generic;

namespace Objforming
{
    public static class RelationalComponentTableExtension
    {
        /// <summary>
        /// <see cref="IRelationalComponent.Overrides(IRelationalComponent)"/> が true を返すとき上書きで追加するメソッド
        /// </summary>
        public static void AddComponent<T>(this List<T> list, T component)
            where T : IRelationalComponent
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (component.Overrides(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
            list.Add(component);
        }

        /// <summary>
        /// <see cref="IRelationalComponent.Overrides(IRelationalComponent)"/> が true を返すとき上書きで追加するメソッド
        /// </summary>
        public static void AddComponents<T>(this List<T> list, IEnumerable<T> components)
            where T : IRelationalComponent
        {
            foreach (var component in components)
            {
                list.AddComponent(component);
            }
        }
    }
}
