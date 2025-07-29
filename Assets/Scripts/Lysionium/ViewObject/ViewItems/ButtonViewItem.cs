using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Items/LUI Button View Item")]
    [RequireComponent(typeof(Button))]
    public class ButtonViewItem : ViewItem
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private KeyIcon _keyIcon = null;
        private Action<InputAction.CallbackContext> clickActionPerformed;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";
        private Animator animator;

        private IButtonViewItemHandler handler;
        private object item;
        private string style;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                handler.HandleClick(item, Manager, Arg);
            });

            clickActionPerformed = ctx => ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

            TryGetComponent(out animator);
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            this.handler = handler as IButtonViewItemHandler;
            this.item = item;

            if (_text != null)
            {
                _text.text = Manager.Localize(name);
            }

            if (_icon != null && handler is IColoredIconViewItemHandler iconViewItemHandler)
            {
                iconViewItemHandler.GetIcon(item, Manager, Arg, out var iconSprite, out var iconColor);
                iconSprite = Manager.Localize(iconSprite);
                if (iconSprite != null)
                {
                    _icon.sprite = iconSprite;
                    _icon.color = iconColor;
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

            var newStyle = handler.GetStyle(item, Manager, Arg);
            if (newStyle == null) { newStyle = _defaultStyle; }
            SetStyle(newStyle);
        }

        private void SetStyle(string newStyle)
        {
            if (newStyle == style) return;

            // この値が true のとき新しいスタイルの適用、 false のとき設定済みスタイルの初期化
            var apply = newStyle != null;

            if (apply && style != null) throw new InvalidOperationException($"スタイル ({style}) 解除前に新しいスタイルを適用することはできません。");

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
                        var any = false;
                        for (int j = 0; j < animator.layerCount; j++)
                        {
                            if (EqualsIgnoreWhiteSpace(animator.GetLayerName(j), styleItem))
                            {
                                // スタイル名と一致するレイヤーの重みを更新する
                                var weight = apply ? 1f : 0f;
                                animator.SetLayerWeight(j, weight);
                                any = true;
                            }
                        }

                        if (apply && !any)
                        {
                            // レイヤーが見つからなければ警告
                            Debug.LogWarning($"レイヤー {new string(styleItem)} が見つかりませんでした。存在するレイヤー: {string.Join(", ", GetLayerNames(animator))}");
                        }
                    }

                    // キーバインド
                    if (styleItem.StartsWith("click:"))
                    {
                        if (apply)
                        {
                            Parent.KeyBind(styleItem.Slice("click:".Length), clickActionPerformed);
                            if (_keyIcon != null && Parent.TryGetKeyIcon(styleItem.Slice("click:".Length), out var keyText, out var keySprite))
                            {
                                _keyIcon.SetKeyIcon(keyText, keySprite);
                            }
                        }
                        else
                        {
                            Parent.Unbind(styleItem.Slice("click:".Length), clickActionPerformed);
                            if (_keyIcon != null)
                            {
                                _keyIcon.ClearKeyIcon();
                            }
                        }
                    }
                }
            }

            // 設定済みスタイルを破棄
            if (!apply) { style = null; }
        }

        private static bool EqualsIgnoreWhiteSpace(string layerName, ReadOnlySpan<char> style)
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

        private static IEnumerable<string> GetLayerNames(Animator animator)
        {
            for (int i = 0; i < animator.layerCount; i++)
            {
                yield return animator.GetLayerName(i);
            }
        }

        private void OnDestroy()
        {
            // キーバインドを解除するためにスタイルをリセット
            SetStyle(null);
        }
    }
}
