using TMPro;
using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Items/LUI Label View Item")]
    [RequireComponent(typeof(TMP_Text))]
    public class LabelViewItem : ViewItem
    {
        private TMP_Text text;
        [SerializeField] private KeyIcon _keyIcon = null;
        private Animator animator;
        private ViewItemStyleEvaluator styleEvaluator;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";

        protected virtual void Awake()
        {
            TryGetComponent(out text);

            TryGetComponent(out animator);
            styleEvaluator = new ViewItemStyleEvaluator();
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            if (text != null)
            {
                text.text = Manager.Localize(name);
            }

            var style = handler.GetStyle(item, Manager, Arg) ?? _defaultStyle;
            styleEvaluator.SetParameters(item, handler, Manager, Arg, Parent);
            styleEvaluator.SetStyle(style, animator, _keyIcon);
        }

        protected override void UnbindCore(object item, IViewItemHandler handler)
        {
            if (text != null)
            {
                text.text = null;
            }

            styleEvaluator.ResetStyle(animator, _keyIcon);
        }
    }
}
