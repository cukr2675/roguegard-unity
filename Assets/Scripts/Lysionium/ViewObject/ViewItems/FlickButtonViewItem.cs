using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [RequireComponent(typeof(Selectable))]
    public class FlickButtonViewItem : ViewItem, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private KeybindLabel _keybindLabel = null;
        private Selectable _selectable;
        private Animator animator;
        private System.Action<InputAction.CallbackContext> inputPerformed;
        private System.Action<InputAction.CallbackContext> inputCanceled;
        private ViewItemStyleEvaluator styleEvaluator;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";

        private IFlickButtonViewItemHandler handler;
        private object item;
        private bool isDown;

        protected virtual void Awake()
        {
            TryGetComponent(out _selectable);
            TryGetComponent(out animator);
            inputPerformed = ctx =>
            {
                if (!_selectable.interactable) return;

                handler.KeyDown(item, Manager);
            };
            inputCanceled = ctx =>
            {
                if (!_selectable.interactable) return;

                Expand();
                handler.KeyUp(item, Manager);
            };
            styleEvaluator = new ViewItemStyleEvaluator();
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            this.handler = handler as IFlickButtonViewItemHandler;
            this.item = item;

            if (_text != null)
            {
                _text.text = Manager.Localize(ItemName);
            }

            var style = handler.GetStyle(item, Manager) ?? _defaultStyle;
            styleEvaluator.Bind(item, handler, Manager, Parent);
            styleEvaluator.SetStyle(style, animator, _keybindLabel, inputPerformed: inputPerformed, inputCanceled: inputCanceled);
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

            styleEvaluator.ResetStyle(animator, _keybindLabel, inputPerformed: inputPerformed, inputCanceled: inputCanceled);
        }

        private void Expand()
        {
            // ドロップダウンの位置を更新
            var rectTransform = (RectTransform)transform;
            var rect = rectTransform.rect;
            rect.position = rectTransform.position;
            Manager.SetInvisibleDropdownPosition(rect);

            handler.Expand(item, Manager);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!_selectable.interactable) return;
            if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行

            isDown = true;
            handler.KeyDown(item, Manager);

            //if (!_expandMethod.HasFlag(ExpandMethod.Press)) return; // 押下で展開する設定のときのみ実行
            //if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行

            //Click();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!isDown) return;
            if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行

            isDown = false;
            handler.KeyUp(item, Manager);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!_selectable.interactable) return;
            if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行
            if (eventData.dragging) return; // 非ドラッグのときのみ実行

            Expand();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!_selectable.interactable) return;
            if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行
            if (eventData.pointerPress != gameObject) return; // フリック時（ポインターが自身を押下したうえで離れたとき）のみ実行

            Expand();
        }
    }
}
