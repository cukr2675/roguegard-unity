using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lysionium.Views
{
    /// <summary>
    /// テキストを表示する <see cref="ViewWidget"/>。リンククリック機能付き
    /// </summary>
    [AddComponentMenu("UI/Lysionium/View Widgets/LUI Label View Widget")]
    public class LabelViewWidget : ViewWidget, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _text = null;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";
        [Space, SerializeField] private Button.ButtonClickedEvent _onClick = null;

        private ILabelWidgetOption widgetOption;
        private SubviewBase _parent;

        protected override SubviewBase Parent => _parent;

        public override bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget viewWidget)
        {
            if (item is string text)
            {
                var labelViewWidget = Instantiate(this, subview.transform);
                labelViewWidget._parent = subview;
                labelViewWidget.Initialize(text);
                viewWidget = labelViewWidget;
                return true;
            }
            else if (item is ILabelWidgetOption widgetOption)
            {
                var baseText = widgetOption.GetText(subview.Manager, subview.Arg);
                var labelViewWidget = Instantiate(this, subview.transform);
                labelViewWidget._parent = subview;
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

            _onClick.Invoke();

            var linkInfo = _text.textInfo.linkInfo[linkIndex];
            widgetOption.ClickLink(linkInfo.GetLinkText(), _parent.Manager, _parent.Arg);
        }
    }
}
