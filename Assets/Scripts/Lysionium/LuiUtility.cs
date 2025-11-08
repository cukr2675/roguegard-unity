using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Lysionium
{
    public static class LuiUtility
    {
        private static int identity = 0;

        public static string EmitIdentity(string header) => $"{header}({identity++})";

        internal static bool TryGetComponentInParent<T>(Transform transform, out T component)
            where T : Component
        {
            if (transform == null)
            {
                component = null;
                return false;
            }

            component = transform.GetComponentInParent<T>();
            return component != null;
        }

        private static EventSystem GetEventSystem(Transform transform)
        {
            if (transform == null) return null;

            if (TryGetComponentInParent<PlayerInput>(transform, out var playerInput) && playerInput.uiInputModule)
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
