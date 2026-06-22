using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lysionium.Views
{
    public class ViewItemStyleEvaluator
    {
        private IInputActionViewItemHandler inputActionHandler;
        private object item;
        private IListuiManager manager;
        private SubviewBase subview;
        private readonly Action<InputAction.CallbackContext> inputStarted;
        private readonly Action<InputAction.CallbackContext> inputPerformed;
        private readonly Action<InputAction.CallbackContext> inputCanceled;

        public string CurrentStyle { get; private set; }

        public ViewItemStyleEvaluator()
        {
            inputStarted = ctx =>
            {
                if (subview.Interactable) { inputActionHandler?.Started(item, manager, ctx); }
            };
            inputPerformed = ctx =>
            {
                if (subview.Interactable) { inputActionHandler?.Performed(item, manager, ctx); }
            };
            inputCanceled = ctx =>
            {
                if (subview.Interactable) { inputActionHandler?.Canceled(item, manager, ctx); }
            };
        }

        public void Bind(object item, IViewItemHandler handler, IListuiManager manager, SubviewBase subview)
        {
            this.item = item;
            inputActionHandler = handler as IInputActionViewItemHandler;
            this.manager = manager;
            this.subview = subview;
        }

        public void SetStyle(
            string style, Animator animator, KeybindLabel keybindLabel, Action<InputAction.CallbackContext> onClick = null,
            Action<InputAction.CallbackContext> inputStarted = null,
            Action<InputAction.CallbackContext> inputPerformed = null,
            Action<InputAction.CallbackContext> inputCanceled = null)
        {
            if (style == null) throw new ArgumentNullException(nameof(style));
            if (style == CurrentStyle) return;

            if (CurrentStyle != null) throw new InvalidOperationException(
                $"スタイル ({style}) 解除前に新しいスタイル ({style}) を適用することはできません。");

            Evaluate(style, true, animator, keybindLabel, onClick, inputStarted, inputPerformed, inputCanceled);

            // 新しいスタイルを保持
            CurrentStyle = style;
        }

        public void ResetStyle(
            Animator animator, KeybindLabel keybindLabel, Action<InputAction.CallbackContext> onClick = null,
            Action<InputAction.CallbackContext> inputStarted = null,
            Action<InputAction.CallbackContext> inputPerformed = null,
            Action<InputAction.CallbackContext> inputCanceled = null)
        {
            if (CurrentStyle == null) return;

            Evaluate(CurrentStyle, false, animator, keybindLabel, onClick, inputStarted, inputPerformed, inputCanceled);

            // 設定済みスタイルを破棄
            CurrentStyle = null;
        }

        private void Evaluate(
            string style, bool apply, Animator animator, KeybindLabel keybindLabel, Action<InputAction.CallbackContext> onClick,
            Action<InputAction.CallbackContext> inputStarted,
            Action<InputAction.CallbackContext> inputPerformed,
            Action<InputAction.CallbackContext> inputCanceled)
        {
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
                            Debug.LogWarning(
                                $"レイヤー {new string(styleItem)} が見つかりませんでした。存在するレイヤー: {string.Join(", ", GetLayerNames(animator))}");
                        }
                    }

                    // click specifier (クリック指定子): キーバインド
                    // ViewItem のバインドで行う関係上 ViewItem が仮想化スクロールでバインド解除されるとキーバインドも解除されてしまうので、
                    // スクロールビューでキーバインドを使用する場合は仮想化を切る必要がある。
                    // Subview 単位でキーバインドすることでも解決できるが、ボタンクリックアニメーションの呼び出しが複雑かつ不確実になるため実装しない。
                    if (styleItem.StartsWith("click:"))
                    {
                        if (apply)
                        {
                            subview.Keybind(styleItem["click:".Length..], onClick);
                            if (keybindLabel != null) { keybindLabel.SetStyle(styleItem["click:".Length..]); }
                        }
                        else
                        {
                            subview.Keyunbind(styleItem["click:".Length..], onClick);
                            if (keybindLabel != null) { keybindLabel.ResetStyle(styleItem["click:".Length..]); }
                        }
                    }

                    // input 指定子
                    if (styleItem.StartsWith("input:"))
                    {
                        if (apply)
                        {
                            subview.Keybind(styleItem["input:".Length..], this.inputStarted, this.inputPerformed, this.inputCanceled);
                            subview.Keybind(styleItem["input:".Length..], inputStarted, inputPerformed, inputCanceled);
                            if (keybindLabel != null) { keybindLabel.SetStyle(styleItem["click:".Length..]); }
                        }
                        else
                        {
                            subview.Keyunbind(styleItem["input:".Length..], this.inputStarted, this.inputPerformed, this.inputCanceled);
                            subview.Keyunbind(styleItem["input:".Length..], inputStarted, inputPerformed, inputCanceled);
                            if (keybindLabel != null) { keybindLabel.ResetStyle(styleItem["click:".Length..]); }
                        }
                    }

                    // preview 指定子: KeyIcon を表示のみ変更
                    if (styleItem.StartsWith("preview:"))
                    {
                        if (apply)
                        {
                            if (keybindLabel != null) { keybindLabel.SetStyle(styleItem["click:".Length..]); }
                        }
                        else
                        {
                            if (keybindLabel != null) { keybindLabel.ResetStyle(styleItem["click:".Length..]); }
                        }
                    }
                }
            }
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
    }
}
