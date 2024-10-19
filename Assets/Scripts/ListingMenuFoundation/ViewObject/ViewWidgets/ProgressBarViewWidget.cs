using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Widgets/LMF Progress Bar View Widget")]
    public class ProgressBarViewWidget : ViewWidget
    {
        [SerializeField] private Image[] _fillAmountTargetImages = null;
        [SerializeField] private string _fillAmountFloat = null;

        private ElementsSubView parent;
        private IWidgetOption widgetOption;
        private Animator animator;

        public delegate float GetProgressFunc<TMgr, TArg>(TMgr manager, TArg arg);

        public override bool TryInstantiateWidget(
            ElementsSubView elementsSubView, IElementHandler handler, object element, out ViewWidget viewWidget)
        {
            if (!(element is IWidgetOption widgetOption))
            {
                viewWidget = null;
                return false;
            }

            var progressBarViewWidget = Instantiate(this);
            progressBarViewWidget.parent = elementsSubView;
            progressBarViewWidget.widgetOption = widgetOption;
            progressBarViewWidget.animator = progressBarViewWidget.GetComponent<Animator>();
            viewWidget = progressBarViewWidget;
            return true;
        }

        private void Update()
        {
            var fillAmount = widgetOption.GetProgress(parent.Manager, parent.Arg);
            foreach (var image in _fillAmountTargetImages)
            {
                image.fillAmount = fillAmount;
            }
            if (animator != null)
            {
                animator.SetFloat(_fillAmountFloat, fillAmount);
            }
        }

        public static IWidgetOption CreateOption<TMgr, TArg>(GetProgressFunc<TMgr, TArg> getProgress)
        {
            return new WidgetOption<TMgr, TArg>()
            {
                GetProgress = getProgress
            };
        }

        public interface IWidgetOption
        {
            float GetProgress(IListMenuManager manager, IListMenuArg arg);
        }

        private class WidgetOption<TMgr, TArg> : IWidgetOption
        {
            public GetProgressFunc<TMgr, TArg> GetProgress { get; set; }

            float IWidgetOption.GetProgress(IListMenuManager manager, IListMenuArg arg)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return 0f;

                return GetProgress(tMgr, tArg);
            }
        }
    }
}
