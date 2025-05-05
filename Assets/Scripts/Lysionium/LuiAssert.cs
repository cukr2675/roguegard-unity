using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    internal static class LuiAssert
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
                Debug.LogError($"{instance} を {typeof(T)} に変換できません。");
                manager?.ErrorOption.HandleClick(manager, null);

                castedInstance = default;
                return true;
            }
        }

        public static void NotInitialized(object self, bool initialized)
        {
            if (initialized) throw new System.InvalidOperationException($"{self} は初期化済みです。");
        }
    }
}
