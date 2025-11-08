using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.Views
{
    /// <summary>
    /// 複数のウィジェットを横並びに表示する <see cref="ViewWidget"/>
    /// </summary>
    [AddComponentMenu("UI/Lysionium/View Widgets/LUI Stack View Widget")]
    public class StackViewWidget : ViewWidget
    {
        public override bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget stackViewWidget)
        {
            if (item is IStackWidgetOption viewWidgets)
            {
                stackViewWidget = Instantiate(this);
                var content = (RectTransform)stackViewWidget.transform;

                var children = viewWidgets.Children;
                var widths = new float[children.Count];
                var totalWidth = 0f;
                for (int i = 0; i < children.Count; i++)
                {
                    var width = children[i].width;
                    if (!width.EndsWith('*')) throw new System.NotImplementedException();

                    totalWidth += widths[i] = float.Parse(width.AsSpan(0, width.Length - 1));
                }

                var sumWidth = 0f;
                var maxHeight = 0f;
                for (int i = 0; i < children.Count; i++)
                {
                    if (!ViewWidgetFactory.TryCreateViewWidget(children[i].item, handler, subview, out var viewWidget))
                    {
                        Debug.LogError($"{children[i]} の {nameof(ViewWidget)} を生成できません。");
                        continue;
                    }

                    viewWidget.SetParent(content, false);
                    viewWidget.anchorMin = new Vector2(sumWidth / totalWidth, viewWidget.anchorMin.y);
                    sumWidth += widths[i];
                    viewWidget.anchorMax = new Vector2(sumWidth / totalWidth, viewWidget.anchorMax.y);
                    viewWidget.sizeDelta = new Vector2(0f, viewWidget.sizeDelta.y);
                    maxHeight = Mathf.Max(maxHeight, viewWidget.rect.height);
                }
                content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, maxHeight);
                return true;
            }

            if (item is IReadOnlyList<object> oldViewWidgets)
            {
                Debug.LogWarning("Obsolete");

                stackViewWidget = Instantiate(this);
                var content = (RectTransform)stackViewWidget.transform;

                var maxHeight = 0f;
                var viewItemWidth = 1f / oldViewWidgets.Count;
                for (int i = 0; i < oldViewWidgets.Count; i++)
                {
                    if (!ViewWidgetFactory.TryCreateViewWidget(oldViewWidgets[i], handler, subview, out var viewWidget))
                    {
                        Debug.LogError($"{oldViewWidgets[i]} の {nameof(ViewWidget)} を生成できません。");
                        continue;
                    }

                    viewWidget.SetParent(content, false);
                    viewWidget.anchorMin = new Vector2(i * viewItemWidth, viewWidget.anchorMin.y);
                    viewWidget.anchorMax = new Vector2((i + 1) * viewItemWidth, viewWidget.anchorMax.y);
                    viewWidget.sizeDelta = new Vector2(0f, viewWidget.sizeDelta.y);
                    maxHeight = Mathf.Max(maxHeight, viewWidget.rect.height);
                }
                content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, maxHeight);
                return true;
            }

            stackViewWidget = null;
            return false;
        }
    }
}
