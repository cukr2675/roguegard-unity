using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Elements/LMF Button View Element")]
    [RequireComponent(typeof(Button))]
    public class ButtonViewElement : ViewElement
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;
        private Button button;
        private Action<InputAction.CallbackContext> clickActionPerformed;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";
        [Space, SerializeField] private Button.ButtonClickedEvent _onClickWithoutBlock = null;
        private Animator animator;

        private IButtonElementHandler handler;
        private object element;
        private string style;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (IsBlocked) return;

                _onClickWithoutBlock.Invoke();
                handler.HandleClick(element, Manager, Arg);
            });

            clickActionPerformed = ctx => button.onClick.Invoke();

            TryGetComponent(out animator);
        }

        protected override void InnerSetElement(IElementHandler handler, object element)
        {
            this.handler = handler as IButtonElementHandler;
            this.element = element;

            if (_text != null)
            {
                _text.text = Manager.Localize(name);
            }

            if (_icon != null)
            {
                var baseIcon = handler.GetIcon(element, Manager, Arg);
                var icon = Manager.Localize(baseIcon);
                if (icon != null)
                {
                    _icon.sprite = icon;
                    _icon.SetNativeSize();
                    _icon.enabled = true;
                }
                else
                {
                    _icon.sprite = null;
                    _icon.enabled = false;
                }
            }

            // 前回のスタイルを解除する
            SetStyle(null);

            var newStyle = handler.GetStyle(element, Manager, Arg);
            if (newStyle == null) { newStyle = _defaultStyle; }
            SetStyle(newStyle);
        }

        private void SetStyle(string newStyle)
        {
            if (newStyle == style) return;

            // この値が true のとき新しいスタイルの適用、 false のとき設定済みスタイルの初期化
            var apply = newStyle != null;

            // 新しいスタイルを保持
            if (apply) { style = newStyle; }

            // スタイルをスペース区切りで処理する
            for (int i = 0; i < style.Length; i++)
            {
                if ((i == 0 || style[i - 1] == ' ') && style[i] != ' ')
                {
                    var styleItemStart = i;
                    var styleItemLength = style.IndexOf(' ', styleItemStart);
                    if (styleItemLength == -1) { styleItemLength = style.Length - styleItemStart; }
                    i = styleItemStart + styleItemLength;

                    // スペース区切りで取得したスタイル名
                    var styleItem = style.AsSpan(styleItemStart, styleItemLength);

                    // AnimationController のレイヤーの重みをスタイル名で変更する
                    if (!styleItem.Contains(":".AsSpan(), StringComparison.CurrentCulture) && animator != null)
                    {
                        for (int j = 0; j < animator.layerCount; j++)
                        {
                            if (EqualsIgnoreWhiteSpace(animator.GetLayerName(j), styleItem))
                            {
                                // スタイル名と一致するレイヤーの重みを更新する
                                var weight = apply ? 1f : 0f;
                                animator.SetLayerWeight(j, weight);
                            }
                        }
                    }

                    // キーバインド
                    if (styleItem.StartsWith("click:"))
                    {
                        if (apply) { Parent.Bind(styleItem.Slice("click:".Length), clickActionPerformed); }
                        else { Parent.Unbind(styleItem.Slice("click:".Length), clickActionPerformed); }
                    }
                }
            }

            // 設定済みスタイルを破棄
            if (!apply) { style = null; }
        }

        private static bool EqualsIgnoreWhiteSpace(string layerName, System.ReadOnlySpan<char> style)
        {
            var styleIndex = 0;
            for (int i = 0; i < layerName.Length; i++)
            {
                if (layerName[i] == ' ') continue; // レイヤー名の空白はないものとして判定する

                if (layerName[i] != style[styleIndex]) return false;

                styleIndex++;
            }
            return true;
        }

        private void OnDestroy()
        {
            // キーバインドを解除するためにスタイルをリセット
            SetStyle(null);
        }
    }
}
