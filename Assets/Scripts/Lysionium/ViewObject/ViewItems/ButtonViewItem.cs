using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/View Items/LUI Button View Item")]
    [RequireComponent(typeof(Button))]
    public class ButtonViewItem : ViewItem
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private KeybindLabel _keybindLabel;
        private Animator animator;
        private System.Action<InputAction.CallbackContext> clickActionPerformed;
        private ViewItemStyleEvaluator styleEvaluator;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";

        private IButtonViewItemHandler handler;
        private object item;

        protected virtual void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                // ドロップダウンの位置を更新
                var rectTransform = (RectTransform)transform;
                var rect = rectTransform.rect;
                rect.position = rectTransform.position;
                Manager.SetInvisibleDropdownPosition(rect);

                handler.Click(item, Manager, Arg);
            });

            TryGetComponent(out animator);
            clickActionPerformed = ctx => ExecuteEvents.Execute(gameObject, new BaseEventData(Parent.EventSystem), ExecuteEvents.submitHandler);
            styleEvaluator = new ViewItemStyleEvaluator();
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            this.handler = handler as IButtonViewItemHandler;
            this.item = item;

            if (_text != null)
            {
                _text.text = Manager.Localize(ItemName);
            }

            if (_icon != null && handler is IColoredIconViewItemHandler iconViewItemHandler)
            {
                iconViewItemHandler.GetIcon(item, Manager, Arg, out var iconSprite, out var iconColor);
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

            var style = handler.GetStyle(item, Manager, Arg) ?? _defaultStyle;
            styleEvaluator.Bind(item, handler, Manager, Arg, Parent);
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
