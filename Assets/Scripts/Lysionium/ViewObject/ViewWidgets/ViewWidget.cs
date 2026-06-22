using UnityEngine;

namespace Lysionium.Views
{
    /// <summary>
    /// <see cref="WidgetsSubview"/> 用の要素コンポーネント。 <see cref="ViewItem"/> と違い表示ごとに再生成される
    /// </summary>
    public abstract class ViewWidget : MonoBehaviour
    {
        /// <summary>
        /// このウィジェットが Selectable であればオーバーライドする
        /// </summary>
        public virtual string WidgetName => null;

        protected virtual SubviewBase Parent => null;

        public abstract bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget viewWidget);

        public void PlayEvtfxString(string value) => Parent.PlayEvtfxFromItem(value, this);
        public void PlayEvtfxObject(Object value) => Parent.PlayEvtfxFromItem(value, this);
    }
}
