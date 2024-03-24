using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objforming.Unity.RuntimeInspector
{
    public abstract class LinkRelationalForm : RelationalForm
    {
        private readonly LinkElement linkElementPrefab;

        protected LinkRelationalForm(LinkElement linkElementPrefab)
        {
            this.linkElementPrefab = linkElementPrefab;
        }

        public sealed override void AppendElementTo(FormInspector inspector, string key, ElementValueGetter getter, ElementValueSetter setter)
        {
            // デフォルトの実装ではこのフォームへのリンクを表示する
            var linkElement = Object.Instantiate(linkElementPrefab, inspector.Page);
            var value = getter();
            linkElement.Initialize(inspector, key, value);
        }
    }
}
