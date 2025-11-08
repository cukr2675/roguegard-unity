using UnityEngine;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/View Widgets/LUI Progress Bar View Widget")]
    public class ProgressBarViewWidget : ViewWidget
    {
        [SerializeField] private Image[] _fillAmountTargetImages = null;
        [SerializeField] private string _fillAmountFloat = null;

        private IProgressBarWidgetOption widgetOption;
        private SubviewBase _parent;
        private Animator animator;

        protected override SubviewBase Parent => _parent;

        public override bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget viewWidget)
        {
            if (item is not IProgressBarWidgetOption widgetOption)
            {
                viewWidget = null;
                return false;
            }

            var progressBarViewWidget = Instantiate(this);
            progressBarViewWidget._parent = subview;
            progressBarViewWidget.widgetOption = widgetOption;
            progressBarViewWidget.animator = progressBarViewWidget.GetComponent<Animator>();
            viewWidget = progressBarViewWidget;
            return true;
        }

        protected virtual void Update()
        {
            var fillAmount = widgetOption.GetProgress(_parent.Manager, _parent.Arg);
            foreach (var image in _fillAmountTargetImages)
            {
                image.fillAmount = fillAmount;
            }
            if (animator != null)
            {
                animator.SetFloat(_fillAmountFloat, fillAmount);
            }
        }
    }
}
