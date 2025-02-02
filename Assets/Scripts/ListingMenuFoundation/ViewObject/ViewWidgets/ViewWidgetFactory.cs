using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// <see cref="ViewWidget"/> 用ファクトリーコンポーネント。このオブジェクトから下の <see cref="WidgetsSubView"/> に影響を与える
    /// </summary>
    [AddComponentMenu("UI/Listing Menu Foundation/LMF View Widget Factory")]
    public class ViewWidgetFactory : MonoBehaviour
    {
        [SerializeField] private ViewWidget[] _ViewWidgetPrefabs = null;
        [SerializeField] private ViewElement _fallbackViewElementPrefab = null;

        public static bool TryCreateViewWidget(object element, IElementHandler handler, ElementsSubViewBase elementsSubView, out RectTransform viewWidget)
        {
            var transform = elementsSubView.transform;
            while (LMFUtility.TryGetComponentInRecursiveParents<ViewWidgetFactory>(transform, out var library))
            {
                if (library.TryCreate(element, handler, elementsSubView, out viewWidget)) return true;

                transform = library.transform.parent;
            }
            viewWidget = null;
            return false;
        }

        private bool TryCreate(object element, IElementHandler handler, ElementsSubViewBase elementsSubView, out RectTransform viewWidget)
        {
            foreach (var viewWidgetPrefab in _ViewWidgetPrefabs)
            {
                if (viewWidgetPrefab.TryInstantiateWidget(element, handler, elementsSubView, out var widget))
                {
                    viewWidget = (RectTransform)widget.transform;
                    return true;
                }
            }
            if (_fallbackViewElementPrefab != null)
            {
                var viewElement = Instantiate(_fallbackViewElementPrefab);
                viewElement.Initialize(elementsSubView);
                viewElement.SetElement(element, handler);
                viewWidget = (RectTransform)viewElement.transform;
                return true;
            }
            viewWidget = null;
            return false;
        }
    }
}
