using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Lysionium
{
    public static class LuiUtility
    {
        /// <summary>
        /// <paramref name="transform"/> の親の <typeparamref name="T"/> 型のコンポーネントを再帰検索する
        /// </summary>
        internal static bool TryGetComponentInRecursiveParents<T>(Transform transform, out T component)
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

        private static EventSystem GetEventSystem(Transform transform)
        {
            if (transform == null) return null;

            if (TryGetComponentInRecursiveParents<PlayerInput>(transform, out var playerInput) && playerInput.uiInputModule)
            {
                // PlayerInput が存在するかつ uiInputModule が設定されている場合は同一オブジェクトの EventSystem を使用する（MultiplayerEventSystem 対策）
                return playerInput.uiInputModule.GetComponent<EventSystem>();
            }
            else
            {
                return EventSystem.current;
            }
        }

        public static EventSystem GetEventSystem(GameObject obj)
        {
            return GetEventSystem(obj.transform);
        }

        public static EventSystem GetEventSystem(Component component)
        {
            return GetEventSystem(component.transform);
        }
    }
}
