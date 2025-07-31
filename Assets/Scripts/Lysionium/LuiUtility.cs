using UnityEngine;

namespace Lysionium
{
    internal static class LuiUtility
    {
        /// <summary>
        /// <paramref name="transform"/> の親の <typeparamref name="T"/> 型のコンポーネントを再帰検索する
        /// </summary>
        public static bool TryGetComponentInRecursiveParents<T>(Transform transform, out T component)
            where T : Component
        {
            if (transform == null)
            {
                component = null;
                return false;
            }

            component = transform.GetComponent<T>();
            if (component != null) return true;
            else return TryGetComponentInRecursiveParents(transform.parent, out component);
        }
    }
}
