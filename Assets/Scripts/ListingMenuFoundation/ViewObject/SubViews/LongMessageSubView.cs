using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Long Message Sub View")]
    public class LongMessageSubView : MessageBoxSubView
    {
        [SerializeField] private ScrollRect _scrollRect = null;

        private TMP_Text text;

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
        {
            SetArg(manager, arg);
            SetStatusCode(0);
        }

        protected override void Update()
        {
            base.Update();

            if (text == null)
            {
                text = MessageBox.GetComponentInChildren<TMP_Text>();
            }

            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, text.renderedHeight);
        }
    }
}
