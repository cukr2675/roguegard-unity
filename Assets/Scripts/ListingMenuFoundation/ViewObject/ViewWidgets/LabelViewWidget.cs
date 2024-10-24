using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Widgets/LMF Label View Widget")]
    public class LabelViewWidget : ViewWidget, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _text = null;

        private ElementsSubView parent;
        private IWidgetOption widgetOption;

        public override bool TryInstantiateWidget(
            ElementsSubView elementsSubView, IElementHandler handler, object element, out ViewWidget viewWidget)
        {
            if (element is string text)
            {
                var labelViewWidget = Instantiate(this, elementsSubView.transform);
                labelViewWidget.parent = elementsSubView;

                if (string.IsNullOrWhiteSpace(text))
                {
                    labelViewWidget._text.text = null;
                }
                else
                {
                    // •¶Žš—ñ‚ÉƒŠƒ“ƒN‚ð“\‚Á‚½‚à‚Ì‚ð•\Ž¦
                    var baseText = Regex.Replace(text, @"(https?://\S+)", "<color=#8080ff><u><link>$1</link></u></color>");
                    labelViewWidget._text.text = labelViewWidget.parent.Manager.Localize(baseText);

                    labelViewWidget._text.ForceMeshUpdate(true, true);
                    var rectTransform = (RectTransform)labelViewWidget.transform;
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, labelViewWidget._text.renderedHeight);
                }

                viewWidget = labelViewWidget;
                return true;
            }
            else if (element is IWidgetOption widgetOption)
            {
                var labelViewWidget = Instantiate(this, elementsSubView.transform);
                labelViewWidget.parent = elementsSubView;
                labelViewWidget.widgetOption = widgetOption;

                if (string.IsNullOrWhiteSpace(widgetOption.Text))
                {
                    labelViewWidget._text.text = null;
                }
                else
                {
                    // •¶Žš—ñ‚ÉƒŠƒ“ƒN‚ð“\‚Á‚½‚à‚Ì‚ð•\Ž¦
                    var baseText = Regex.Replace(widgetOption.Text, @"(https?://\S+)", "<color=#8080ff><u><link>$1</link></u></color>");
                    labelViewWidget._text.text = labelViewWidget.parent.Manager.Localize(baseText);

                    labelViewWidget._text.ForceMeshUpdate(true, true);
                    var rectTransform = (RectTransform)labelViewWidget.transform;
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, labelViewWidget._text.renderedHeight);
                }

                viewWidget = labelViewWidget;
                return true;
            }
            else
            {
                viewWidget = null;
                return false;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (widgetOption == null) return;

            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);
            if (linkIndex == -1) return;

            var linkInfo = _text.textInfo.linkInfo[linkIndex];
            widgetOption.HandleClickLink(parent.Manager, parent.Arg, linkInfo.GetLinkText());
        }

        public static IWidgetOption CreateOption<TMgr, TArg>(string text, System.Action<TMgr, TArg, string> handleClickLink)
        {
            return new WidgetOption<TMgr, TArg>()
            {
                Text = text,
                HandleClickLink = handleClickLink
            };
        }

        public interface IWidgetOption
        {
            string Text { get; }

            void HandleClickLink(IListMenuManager manager, IListMenuArg arg, string link);
        }

        private class WidgetOption<TMgr, TArg> : IWidgetOption
        {
            public string Text { get; set; }
            public System.Action<TMgr, TArg, string> HandleClickLink { get; set; }

            void IWidgetOption.HandleClickLink(IListMenuManager manager, IListMenuArg arg, string link)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return;

                HandleClickLink(tMgr, tArg, link);
            }
        }
    }
}
