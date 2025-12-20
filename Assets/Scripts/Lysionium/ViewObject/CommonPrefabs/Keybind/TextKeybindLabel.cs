using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lysionium.Views
{
    /// <summary>
    /// スタイル名から '&lt;sprite="{SpriteAsset.name}" name="{スタイル名}"&gt;' に変換して表示する <see cref="KeybindLabel"/> の具象コンポーネント
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI Keybind Label")]
    [RequireComponent(typeof(LayoutElement))]
    public class TextKeybindLabel : KeybindLabel
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Vector2 _padding = Vector2.zero;
        
        private LayoutElement layoutElement;
        private string textHead, textFoot;
        private readonly StringBuilder stringBuilder = new();

        protected virtual void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
            textHead = $"<sprite name=\"";
            textFoot = $"\" tint>";
            gameObject.SetActive(false);

            // スタイルシートが存在する場合はそのスプライトアセットを使用
            var keybindStyleSheet = GetComponentInParent<KeybindStyleSheet>();
            if (keybindStyleSheet && keybindStyleSheet.KeybindGlyphAsset && keybindStyleSheet.KeybindGlyphAsset.SpriteAsset)
            {
                var spriteAssetName = keybindStyleSheet.KeybindGlyphAsset.SpriteAsset.name;
                textHead = $"<sprite=\"{spriteAssetName}\" name=\"";
            }
        }

        public override void SetStyle(System.ReadOnlySpan<char> style)
        {
            stringBuilder.Clear().Append(textHead).Append(style).Append(textFoot);
            _text.SetText(stringBuilder);
            _text.ForceMeshUpdate(true, true);
            layoutElement.preferredWidth = _text.preferredWidth + _padding.x * 2f; // 左右を空けるので2倍
            layoutElement.preferredHeight = _text.preferredHeight + _padding.y * 2f; // 上下を空けるので2倍
            gameObject.SetActive(true);
        }

        public override void ResetStyle(System.ReadOnlySpan<char> style)
        {
            gameObject.SetActive(false);
        }
    }
}
