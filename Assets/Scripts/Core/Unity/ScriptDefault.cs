using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// <see cref="ReferableScript"/> の初期値に相当する値を設定するクラス。
    /// </summary>
    internal class ScriptDefault<T>
    {
        public static T Ref { get; private set; }
    }

    internal class ScriptDefault : ScriptableLoader
    {
        [SerializeField] private ScriptRef<object> _asset = null;

        public override IEnumerator LoadAsync()
        {
            var type = typeof(ScriptDefault<>).MakeGenericType(_asset.GetType());
            var refProperty = type.GetRuntimeProperty("Ref");
            var refValue = refProperty.GetValue(null);
            if (refValue != null) throw new RogueException($"{type}.Ref は上書きできません。");

            refProperty.SetValue(null, _asset);
            yield break;
        }

        public override void TestLoad()
        {
#if UNITY_EDITOR
            var type = typeof(ScriptDefault<>).MakeGenericType(_asset.GetType());
            var refProperty = type.GetRuntimeProperty("Ref");
            var refValue = refProperty.GetValue(null);
            if (refValue != null) throw new RogueException($"{type}.Ref は上書きできません。");

            refProperty.SetValue(null, _asset);
#endif
        }
    }
}
