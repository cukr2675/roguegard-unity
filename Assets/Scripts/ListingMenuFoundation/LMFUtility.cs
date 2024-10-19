using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    internal static class LMFUtility
    {
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
