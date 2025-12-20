using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace Lysionium.Views
{
    [CreateAssetMenu(menuName = "Lysionium/Keybind/Keybind Glyph Source")]
    public class KeybindGlyphSource : ScriptableObject
    {
#if UNITY_EDITOR
        // リスト内の InputControl 属性が正常に動作しないため入力補助として利用する
        [Tooltip("Control Path の入力補助エリア")]
        [InputControl]
        [SerializeField] private string _controlPathExample;
#endif

        [SerializeField] private List<KeyValuePair> _items;

#if UNITY_EDITOR
        [Header("Editor Only")]

        [SerializeField] private AutoSetMethodType _autoSetMethod;

        [Tooltip("エディタでの自動生成用のスプライトシート")]
        [SerializeField] private Texture2D _spriteSheet;

        [Tooltip("エディタでの自動生成用のスプライト名フォーマット")]
        [SerializeField] private string[] _spriteNameFormats;

        [SerializeField] private StringPair[] _spriteNameArgOverrides;
#endif

        public bool TryGetValue(InputControl actualInputControl, out Sprite value)
        {
            foreach (var pair in _items)
            {
                if (InputControlPath.Matches(pair._controlPath, actualInputControl))
                {
                    value = pair._sprite;
                    return value != null; // null は取得失敗扱い（AutoSet で null になってもそのまま使えるようにするため）
                }
            }
            value = default;
            return false;
        }

#if UNITY_EDITOR
        [ContextMenu("Auto Set")]
        private void AutoSet()
        {
            if (_spriteSheet == null) throw new System.InvalidOperationException($"{nameof(_spriteSheet)} が設定されていません。");

            var overrides = new Dictionary<string, string>(
                _spriteNameArgOverrides.Select(pair => new KeyValuePair<string, string>(pair._from, pair._to)));

            string Override(string s) => overrides.TryGetValue(s, out var os) ? os : s;

            static List<string> GetControlPathsRecursive(string layout, string prefix = "", List<string> result = null)
            {
                result ??= new List<string>();
                foreach (var control in InputSystem.LoadLayout(layout).controls)
                {
                    var name = control.name.ToString();
                    name = prefix + name[(name.IndexOf('/') + 1)..];
                    result.Add(name);
                    if (!control.layout.IsEmpty()) { GetControlPathsRecursive(control.layout, name + '/', result); }
                }
                return result;
            }

            var controlPaths = new List<(string fullName, string spriteNameArg)>();
            if (_autoSetMethod.HasFlag(AutoSetMethodType.Keyboard))
            {
                controlPaths.AddRange(GetControlPathsRecursive("Keyboard").Select(s => ($"<Keyboard>/{s}", Override(s))));
            }
            if (_autoSetMethod.HasFlag(AutoSetMethodType.Mouse))
            {
                controlPaths.AddRange(GetControlPathsRecursive("Mouse").Select(s => ($"<Mouse>/{s}", Override(s))));
            }
            if (_autoSetMethod.HasFlag(AutoSetMethodType.Gamepad))
            {
                controlPaths.AddRange(GetControlPathsRecursive("Gamepad").Select(s => ($"<Gamepad>/{s}", Override(s))));
            }

            if (controlPaths.Count == 0) return;

            var spriteSheetPath = UnityEditor.AssetDatabase.GetAssetPath(_spriteSheet);
            var sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath)
                .Where(a => a is Sprite)
                .ToDictionary(a => a.name, a => (Sprite)a);

            _items.Clear();
            _items.AddRange(controlPaths.Select(controlPath => new KeyValuePair
            {
                _controlPath = controlPath.fullName,
                _sprite = _spriteNameFormats
                    .Select(format => string.Format(format, controlPath.spriteNameArg))
                    .Select(name => sprites.TryGetValue(name, out var sprite) ? sprite : null)
                    .FirstOrDefault(sprite => sprite != null)
            }));
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        [System.Serializable]
        private class KeyValuePair
        {
            /// <summary>
            /// <see cref="InputControlPath.Matches"/> の恩恵を受けるためデバイス名で区切るようなことはしない
            /// </summary>
            [SerializeField] public string _controlPath;
            [SerializeField] public Sprite _sprite;
        }

        [System.Flags]
        private enum AutoSetMethodType
        {
            None = 0,
            Keyboard = 1,
            Mouse = 2,
            Gamepad = 4,
        }

        [System.Serializable]
        private struct StringPair
        {
            [SerializeField] public string _from;
            [SerializeField] public string _to;
        }
    }
}
