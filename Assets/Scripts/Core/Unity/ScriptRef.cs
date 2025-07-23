using UnityEngine;

namespace Roguegard
{
    [System.Serializable]
    public class ScriptRef<T>
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.MonoScript _script;
#endif

        [SerializeReference] private T _ref = default;

        public T Ref => _ref;
    }
}
