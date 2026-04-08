using TMPro;
using UnityEngine;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/View Items/LUI Label View Item")]
    public class LabelViewItem : ViewItem
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private KeybindLabel _keybindLabel;
        private Animator animator;
        private ViewItemStyleEvaluator styleEvaluator;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";

        protected virtual void Awake()
        {
            TryGetComponent(out animator);
            styleEvaluator = new ViewItemStyleEvaluator();
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            if (_text != null)
            {
                _text.text = Manager.Localize(name);
            }

            var style = handler.GetStyle(item, Manager) ?? _defaultStyle;
            styleEvaluator.Bind(item, handler, Manager, Parent);
            styleEvaluator.SetStyle(style, animator, _keybindLabel);
        }

        protected override void UnbindCore(object item, IViewItemHandler handler)
        {
            if (_text != null)
            {
                _text.text = null;
            }

            styleEvaluator.ResetStyle(animator, _keybindLabel);
        }
    }
}
