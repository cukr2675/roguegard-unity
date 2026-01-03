using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Lysionium.Views
{
    public class FlickableViewItem : ViewItem, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private KeybindLabel _keybindLabel = null;
        private Animator animator;
        private System.Action<InputAction.CallbackContext> inputPerformed;
        private System.Action<InputAction.CallbackContext> inputCanceled;
        private ViewItemStyleEvaluator styleEvaluator;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";

        private IFlickableViewItemHandler handler;
        private object item;

        protected virtual void Awake()
        {
            TryGetComponent(out animator);
            inputPerformed = ctx =>
            {
                handler.KeyDown(item, Manager, Arg);
            };
            inputCanceled = ctx =>
            {
                Click();
                handler.KeyUp(item, Manager, Arg);
            };
            styleEvaluator = new ViewItemStyleEvaluator();
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            this.handler = handler as IFlickableViewItemHandler;
            this.item = item;

            if (_text != null)
            {
                _text.text = Manager.Localize(ItemName);
            }

            var style = handler.GetStyle(item, Manager, Arg) ?? _defaultStyle;
            styleEvaluator.Bind(item, handler, Manager, Arg, Parent);
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

        private void Click()
        {
            // ドロップダウンの位置を更新
            var rectTransform = (RectTransform)transform;
            var rect = rectTransform.rect;
            rect.position = rectTransform.position;
            Manager.SetInvisibleDropdownPosition(rect);

            handler.Expand(item, Manager, Arg);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行

            handler.KeyDown(item, Manager, Arg);

            //if (!_expandMethod.HasFlag(ExpandMethod.Press)) return; // 押下で展開する設定のときのみ実行
            //if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行

            //Click();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行

            handler.KeyUp(item, Manager, Arg);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行
            if (eventData.dragging) return; // 非ドラッグのときのみ実行

            Click();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return; // 左ボタンのときのみ実行
            if (eventData.pointerPress != gameObject) return; // フリック時（ポインターが自身を押下したうえで離れたとき）のみ実行

            Click();
        }
    }
}
