using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// ビューを変更せずに LMF Play を再生するためのクラス
    /// </summary>
    public class PlayEmptyViewTemplate
    {
        public string EmptySubViewName { get; set; } = StandardSubViewTable.ScrollName;

        public void Play(string value, IListMenuManager manager)
        {
            if (value == null) throw new System.ArgumentNullException(nameof(value));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            if (!(manager.GetSubView(EmptySubViewName) is ElementsSubView elementsSubView)) throw new System.InvalidOperationException(
                $"{EmptySubViewName} の SubView は {nameof(ElementsSubView)} ではありません。");

            elementsSubView.PlayString(value);
        }

        public void Play(Object value, IListMenuManager manager)
        {
            if (value == null) throw new System.ArgumentNullException(nameof(value));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            if (!(manager.GetSubView(EmptySubViewName) is ElementsSubView elementsSubView)) throw new System.InvalidOperationException(
                $"{EmptySubViewName} の SubView は {nameof(ElementsSubView)} ではありません。");

            elementsSubView.PlayObject(value);
        }
    }
}
