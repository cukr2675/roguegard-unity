using System.Text;
using TMPro;
using UnityEngine;

namespace Lysionium.Views
{
    /// <summary>
    /// スタイル名から '&lt;sprite="{SpriteAsset.name}" name="{スタイル名}"&gt;' に変換して表示する <see cref="KeybindLabel"/> の具象コンポーネント
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI Keybind Label")]
    public class TextKeybindLabel : KeybindLabel
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Vector2 _padding = Vector2.zero;
        
        private string textHead, textFoot;
        private readonly StringBuilder stringBuilder = new();

        protected virtual void Awake()
        {
            textHead = $"<sprite name=\"";
            textFoot = $"\" tint>";
            gameObject.SetActive(false);

            // スタイルシートが存在する場合はそのスプライトアセットを使用
            var keybindStyleSheet = GetComponentInParent<KeybindStyleSheet>();
            if (keybindStyleSheet && keybindStyleSheet.KeybindGlyphAsset && keybindStyleSheet.KeybindGlyphAsset.SpriteAsset)
            {
                _text.spriteAsset = keybindStyleSheet.KeybindGlyphAsset.SpriteAsset;

                // スプライトアセット名で指定する方法
                // エディタ上では動作するが、実機では裏でキャッシュされるため
                // スプライトアセットの動的変更ができなくなる
                //var spriteAssetName = keybindStyleSheet.KeybindGlyphAsset.SpriteAsset.name;
                //textHead = $"<sprite=\"{spriteAssetName}\" name=\"";
            }
        }

        public override void SetStyle(System.ReadOnlySpan<char> style)
        {
            stringBuilder.Clear().Append(textHead).Append(style).Append(textFoot);
            _text.SetText(stringBuilder);
            _text.ForceMeshUpdate(true, true);
            gameObject.SetActive(true);
        }

        public override void ResetStyle(System.ReadOnlySpan<char> style)
        {
            gameObject.SetActive(false);
        }
    }
}
