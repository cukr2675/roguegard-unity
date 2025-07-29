using UnityEngine;
using UnityEngine.UI;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Widgets/LUI Progress Bar View Widget")]
    public class ProgressBarViewWidget : ViewWidget
    {
        [SerializeField] private Image[] _fillAmountTargetImages = null;
        [SerializeField] private string _fillAmountFloat = null;

        private IWidgetOption widgetOption;
        private SubviewBase _parent;
        private Animator animator;

        protected override SubviewBase Parent => _parent;

        public delegate float GetProgress<TMgr, TArg>(TMgr manager, TArg arg);

        public override bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget viewWidget)
        {
            if (item is not IWidgetOption widgetOption)
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
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return 0f;

                return GetProgress(tMgr, tArg);
            }
        }
    }
}
