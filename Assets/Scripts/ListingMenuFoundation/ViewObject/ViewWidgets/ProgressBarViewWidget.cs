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

        private IWidgetOption widgetOption;
        private ElementsSubViewBase _parent;
        private Animator animator;

        protected override ElementsSubViewBase Parent => _parent;

        public delegate float GetProgress<TMgr, TArg>(TMgr manager, TArg arg);

        public override bool TryInstantiateWidget(
            object element, IElementHandler handler, ElementsSubViewBase elementsSubView, out ViewWidget viewWidget)
        {
            if (!(element is IWidgetOption widgetOption))
            {
                viewWidget = null;
                return false;
            }

            var progressBarViewWidget = Instantiate(this);
            progressBarViewWidget._parent = elementsSubView;
            progressBarViewWidget.widgetOption = widgetOption;
            progressBarViewWidget.animator = progressBarViewWidget.GetComponent<Animator>();
            viewWidget = progressBarViewWidget;
            return true;
        }

        private void Update()
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

        public static IWidgetOption CreateOption<TMgr, TArg>(GetProgress<TMgr, TArg> getProgress, string name = null)
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
            public string Name { get; set; }
            public GetProgress<TMgr, TArg> GetProgress { get; set; }

            float IWidgetOption.GetProgress(IListMenuManager manager, IListMenuArg arg)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return 0f;

                return GetProgress(tMgr, tArg);
            }
        }
    }
}
