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

        public void PlayString(string value) => Parent.PlayFromItem(value, this);
        public void PlayObject(Object value) => Parent.PlayFromItem(value, this);
    }
}
