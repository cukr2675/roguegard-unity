using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class PlayEmptyViewTemplate
    {
        public string EmptySubViewName { get; set; } = StandardSubViewTable.ScrollName;

        public void Play(string value, IListMenuManager manager)
        {
            if (value == null) throw new System.ArgumentNullException(nameof(value));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            if (!(manager.GetSubView(EmptySubViewName) is ElementsSubView elementsSubView)) throw new System.InvalidOperationException(
                $"{EmptySubViewName} ÇÃ SubView ÇÕ {nameof(ElementsSubView)} Ç≈ÇÕÇ†ÇËÇ‹ÇπÇÒÅB");

            elementsSubView.PlayString(value);
        }

        public void Play(Object value, IListMenuManager manager)
        {
            if (value == null) throw new System.ArgumentNullException(nameof(value));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            if (!(manager.GetSubView(EmptySubViewName) is ElementsSubView elementsSubView)) throw new System.InvalidOperationException(
                $"{EmptySubViewName} ÇÃ SubView ÇÕ {nameof(ElementsSubView)} Ç≈ÇÕÇ†ÇËÇ‹ÇπÇÒÅB");

            elementsSubView.PlayObject(value);
        }
    }
}
