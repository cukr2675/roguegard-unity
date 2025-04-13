using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Lysionium
{
    /// <summary>
    /// <see cref="WidgetsSubView"/> 用の要素コンポーネント。 <see cref="ViewElement"/> と違い表示ごとに再生成される
    /// </summary>
    public abstract class ViewWidget : MonoBehaviour
    {
        /// <summary>
        /// このウィジェットが Selectable であればオーバーライドする
        /// </summary>
        public virtual string WidgetName => null;

        protected virtual ElementsSubViewBase Parent => null;

        private static int widgetIdentity = 0;

        public abstract bool TryInstantiateWidget(
            object element, IElementHandler handler, ElementsSubViewBase elementsSubView, out ViewWidget viewWidget);

        protected static string EmitIdentity(string header) => $"{header}({widgetIdentity++})";

        public void PlayString(string value) => Parent.PlayFromElement(value, this);
        public void PlayObject(Object value) => Parent.PlayFromElement(value, this);
    }
}
