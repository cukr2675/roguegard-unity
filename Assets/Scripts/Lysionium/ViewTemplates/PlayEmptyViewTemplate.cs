using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// ビューを変更せずに LUI Play を再生するためのクラス
    /// </summary>
    public class PlayEmptyViewTemplate
    {
        public string EmptySubviewName { get; set; } = StandardSubviewTable.ScrollName;

        public void Play(string value, IListMenuManager manager)
        {
            if (value == null) throw new System.ArgumentNullException(nameof(value));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            if (!(manager.GetSubview(EmptySubviewName) is ElementsSubview elementsSubview)) throw new System.InvalidOperationException(
                $"{EmptySubviewName} の Subview は {nameof(ElementsSubview)} ではありません。");

            elementsSubview.PlayString(value);
        }

        public void Play(Object value, IListMenuManager manager)
        {
            if (value == null) throw new System.ArgumentNullException(nameof(value));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            if (!(manager.GetSubview(EmptySubviewName) is ElementsSubview elementsSubview)) throw new System.InvalidOperationException(
                $"{EmptySubviewName} の Subview は {nameof(ElementsSubview)} ではありません。");

            elementsSubview.PlayObject(value);
        }
    }
}
