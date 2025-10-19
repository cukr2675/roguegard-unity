using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// <see cref="ViewWidget"/> 用ファクトリーコンポーネント。このオブジェクトから下の <see cref="WidgetsSubview"/> に影響を与える
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI View Widget Factory")]
    public class ViewWidgetFactory : MonoBehaviour
    {
        [SerializeField] private ViewWidget[] _ViewWidgetPrefabs = null;
        [SerializeField] private ViewItem _fallbackViewItemPrefab = null;

        public static bool TryCreateViewWidget(object item, IViewItemHandler handler, SubviewBase subview, out RectTransform viewWidget)
        {
            var transform = subview.transform;
            while (LuiUtility.TryGetComponentInParent<ViewWidgetFactory>(transform, out var library))
            {
                if (library.TryCreate(item, handler, subview, out viewWidget)) return true;

                transform = library.transform.parent;
            }
            viewWidget = null;
            return false;
        }

        private bool TryCreate(object item, IViewItemHandler handler, SubviewBase subview, out RectTransform viewWidget)
        {
            foreach (var viewWidgetPrefab in _ViewWidgetPrefabs)
            {
                if (viewWidgetPrefab.TryInstantiateWidget(item, handler, subview, out var widget))
                {
                    viewWidget = (RectTransform)widget.transform;
                    return true;
                }
            }
            if (_fallbackViewItemPrefab != null)
            {
                var viewItem = Instantiate(_fallbackViewItemPrefab);
                viewItem.Initialize(subview);
                viewItem.Bind(item, handler);
                viewWidget = (RectTransform)viewItem.transform;
                return true;
            }
            viewWidget = null;
            return false;
        }
    }
}
