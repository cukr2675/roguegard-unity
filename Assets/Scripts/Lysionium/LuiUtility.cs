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
                if (!playerInput.uiInputModule.TryGetComponent<EventSystem>(out var result))
                    throw new System.InvalidOperationException($"{playerInput}.uiInputModule の {nameof(EventSystem)} が見つかりません。");

                return result;
            }
            else
            {
                var result = EventSystem.current;
                if (result == null) throw new System.InvalidOperationException($"{nameof(EventSystem)} が見つかりません。");

                return result;
            }
        }

        /// <summary>
        /// <see cref="EventSystem.current"/> を使用するため Awake() での呼び出しは非推奨
        /// </summary>
        public static EventSystem GetEventSystem(GameObject obj)
        {
            return GetEventSystem(obj.transform);
        }

        /// <summary>
        /// <see cref="EventSystem.current"/> を使用するため Awake() での呼び出しは非推奨
        /// </summary>
        public static EventSystem GetEventSystem(Component component)
        {
            return GetEventSystem(component.transform);
        }
    }
}
