using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    internal static class LMFAssert
    {
        public static bool Type<T>(object instance, out T castedInstance, IListMenuManager manager = null)
        {
            if (instance is T tInstance)
            {
                castedInstance = tInstance;
                return false;
            }
            else if (instance == null)
            {
                castedInstance = default;
                return false;
            }
            else
            {
                Debug.LogError($"{instance} ‚ğ {typeof(T)} ‚É•ÏŠ·‚Å‚«‚Ü‚¹‚ñB");
                manager?.ErrorOption.HandleClick(manager, null);

                castedInstance = default;
                return true;
            }
        }

        public static void NotInitialized(object self, bool initialized)
        {
            if (initialized) throw new System.InvalidOperationException($"{self} ‚Í‰Šú‰»Ï‚İ‚Å‚·B");
        }
    }
}
