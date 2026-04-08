using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/View Items/LUI Dropdown Button View Item")]
    [RequireComponent(typeof(Button))]
    public class DropdownButtonViewItem : ViewItem
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private KeybindLabel _keybindLabel;
        private Animator animator;
        private System.Action<InputAction.CallbackContext> clickActionPerformed;
        private ViewItemStyleEvaluator styleEvaluator;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";

        private ExpandHandler expandHandler;
        private IButtonViewItemHandler handler;
        private ITreeViewItemHandler treeHandler;
        private object item;

        public delegate void ExpandHandler(IReadOnlyList<object> children, Rect rect);

        protected virtual void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                var rectTransform = (RectTransform)transform;
                var children = treeHandler.GetChildren(item, Manager);
                if (children != null && children.Count >= 1)
                {
                    expandHandler(children, rectTransform.rect);
                    return;
                }

                // ドロップダウンの位置を更新
                Manager.SetInvisibleDropdownPosition(rectTransform.rect);

                handler.Click(item, Manager);
            });

            TryGetComponent(out animator);
            clickActionPerformed = ctx => ExecuteEvents.Execute(gameObject, new BaseEventData(Parent.EventSystem), ExecuteEvents.submitHandler);
            styleEvaluator = new ViewItemStyleEvaluator();
        }

        public void Initialize(SubviewBase parent, ExpandHandler expandHandler)
        {
            Initialize(parent);
            this.expandHandler = expandHandler;
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            this.handler = handler as IButtonViewItemHandler;
            treeHandler = handler as ITreeViewItemHandler;
            this.item = item;

            if (_text != null)
            {
                _text.text = Manager.Localize(ItemName);
            }

            if (_icon != null && handler is IColoredIconViewItemHandler iconViewItemHandler)
            {
                iconViewItemHandler.GetIcon(item, Manager, out var iconSprite, out var iconColor);
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

            var style = handler.GetStyle(item, Manager) ?? _defaultStyle;
            styleEvaluator.Bind(item, handler, Manager, Parent);
            styleEvaluator.SetStyle(style, animator, _keybindLabel, clickActionPerformed);
        }

        protected override void UnbindCore(object item, IViewItemHandler handler)
        {
            this.handler = null;
            this.item = null;

            if (_text != null)
            {
                _text.text = null;
            }
            if (_icon != null)
            {
                _icon.sprite = null;
                _icon.enabled = false;
            }

            styleEvaluator.ResetStyle(animator, _keybindLabel, clickActionPerformed);
        }
    }
}
