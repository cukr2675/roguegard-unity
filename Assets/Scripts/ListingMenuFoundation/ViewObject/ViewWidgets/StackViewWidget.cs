using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// 複数のウィジェットを横並びに表示する <see cref="ViewWidget"/>
    /// </summary>
    [AddComponentMenu("UI/Listing Menu Foundation/View Widgets/LMF Stack View Widget")]
    public class StackViewWidget : ViewWidget
    {
        public override bool TryInstantiateWidget(
            ElementsSubViewBase elementsSubView, IElementHandler handler, object element, out ViewWidget stackViewWidget)
        {
            if (!(element is IReadOnlyList<object> viewWidgets))
            {
                stackViewWidget = null;
                return false;
            }

            stackViewWidget = Instantiate(this);
            var content = (RectTransform)stackViewWidget.transform;

            var maxHeight = 0f;
            var viewElementWidth = 1f / viewWidgets.Count;
            for (int i = 0; i < viewWidgets.Count; i++)
            {
                if (!ViewWidgetFactory.TryCreateViewWidget(elementsSubView, handler, viewWidgets[i], out var viewWidget))
                {
                    Debug.LogError($"{viewWidgets[i]} の {nameof(ViewWidget)} を生成できません。");
                    continue;
                }

                viewWidget.SetParent(content, false);
                viewWidget.anchorMin = new Vector2(i * viewElementWidth, viewWidget.anchorMin.y);
                viewWidget.anchorMax = new Vector2((i + 1) * viewElementWidth, viewWidget.anchorMax.y);
                viewWidget.sizeDelta = new Vector2(0f, viewWidget.sizeDelta.y);
                maxHeight = Mathf.Max(maxHeight, viewWidget.rect.height);
            }
            content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, maxHeight);
            return true;
        }
    }
}
