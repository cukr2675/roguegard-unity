using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF View Widget Factory")]
    public class ViewWidgetFactory : MonoBehaviour
    {
        [SerializeField] private ViewWidget[] _ViewWidgetPrefabs = null;
        [SerializeField] private ViewElement _fallbackViewElementPrefab = null;

        public static bool TryCreateViewWidget(ElementsSubView elementsSubView, IElementHandler handler, object element, out RectTransform viewWidget)
        {
            var transform = elementsSubView.transform;
            while (LMFUtility.TryGetComponentInRecursiveParents<ViewWidgetFactory>(transform, out var library))
            {
                if (library.TryCreate(elementsSubView, handler, element, out viewWidget)) return true;

                transform = library.transform.parent;
            }
            viewWidget = null;
            return false;
        }

        private bool TryCreate(ElementsSubView elementsSubView, IElementHandler handler, object element, out RectTransform viewWidget)
        {
            foreach (var viewWidgetPrefab in _ViewWidgetPrefabs)
            {
                if (viewWidgetPrefab.TryInstantiateWidget(elementsSubView, handler, element, out var widget))
                {
                    viewWidget = (RectTransform)widget.transform;
                    return true;
                }
            }
            if (_fallbackViewElementPrefab != null)
            {
                var viewElement = Instantiate(_fallbackViewElementPrefab);
                viewElement.Initialize(elementsSubView);
                viewElement.SetElement(handler, element);
                viewWidget = (RectTransform)viewElement.transform;
                return true;
            }
            viewWidget = null;
            return false;
        }
    }
}
