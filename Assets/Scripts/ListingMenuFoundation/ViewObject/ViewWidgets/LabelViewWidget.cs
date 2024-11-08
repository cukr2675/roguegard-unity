using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Widgets/LMF Label View Widget")]
    public class LabelViewWidget : ViewWidget, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _text = null;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";
        [Space, SerializeField] private Button.ButtonClickedEvent _onClickWithoutBlock = null;

        private ElementsSubViewBase _parent;
        protected override ElementsSubViewBase Parent => _parent;

        private IWidgetOption widgetOption;

        public override bool TryInstantiateWidget(
            ElementsSubViewBase elementsSubView, IElementHandler handler, object element, out ViewWidget viewWidget)
        {
            if (element is string text)
            {
                var labelViewWidget = Instantiate(this, elementsSubView.transform);
                labelViewWidget._parent = elementsSubView;
                labelViewWidget.Initialize(text);
                viewWidget = labelViewWidget;
                return true;
            }
            else if (element is IWidgetOption widgetOption)
            {
                var baseText = widgetOption.GetText(elementsSubView.Manager, elementsSubView.Arg);
                var labelViewWidget = Instantiate(this, elementsSubView.transform);
                labelViewWidget._parent = elementsSubView;
                labelViewWidget.widgetOption = widgetOption;
                labelViewWidget.Initialize(baseText);
                viewWidget = labelViewWidget;
                return true;
            }
            else
            {
                viewWidget = null;
                return false;
            }
        }

        private void Initialize(string text)
        {
            if (TryGetComponent<Animator>(out var animator))
            {
                for (int i = 0; i < animator.layerCount; i++)
                {
                    if (animator.GetLayerName(i) != _defaultStyle) continue;

                    animator.SetLayerWeight(i, 1f);
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                _text.text = null;
            }
            else
            {
                // 文字列をローカライズして表示
                _text.text = _parent.Manager.Localize(text);

                _text.ForceMeshUpdate(true, true);
                var rectTransform = (RectTransform)transform;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _text.renderedHeight);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (widgetOption == null) return;

            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);
            if (linkIndex == -1) return;

            _onClickWithoutBlock.Invoke();

            var linkInfo = _text.textInfo.linkInfo[linkIndex];
            widgetOption.HandleClickLink(linkInfo.GetLinkText(), _parent.Manager, _parent.Arg);
        }

        public static IWidgetOption CreateOption<TMgr, TArg>(string text, HandleClickElement<string, TMgr, TArg> onClickLink = null)
        {
            return new WidgetOption<TMgr, TArg>()
            {
                GetText = delegate { return text; },
                HandleClickLink = onClickLink
            };
        }

        public static IWidgetOption CreateOption<TMgr, TArg>(GetElementName<TMgr, TArg> getText, HandleClickElement<string, TMgr, TArg> onClickLink = null)
        {
            return new WidgetOption<TMgr, TArg>()
            {
                GetText = getText,
                HandleClickLink = onClickLink
            };
        }

        public interface IWidgetOption
        {
            string GetText(IListMenuManager manager, IListMenuArg arg);

            void HandleClickLink(string link, IListMenuManager manager, IListMenuArg arg);
        }

        private class WidgetOption<TMgr, TArg> : IWidgetOption
        {
            public GetElementName<TMgr, TArg> GetText { get; set; }
            public HandleClickElement<string, TMgr, TArg> HandleClickLink { get; set; }

            string IWidgetOption.GetText(IListMenuManager manager, IListMenuArg arg)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

                return GetText(tMgr, tArg);
            }

            void IWidgetOption.HandleClickLink(string link, IListMenuManager manager, IListMenuArg arg)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return;

                HandleClickLink?.Invoke(link, tMgr, tArg);
            }
        }
    }
}
