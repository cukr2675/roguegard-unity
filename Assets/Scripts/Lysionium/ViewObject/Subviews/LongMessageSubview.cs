using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Long Message Subview")]
    public class LongMessageSubview : MessageBoxSubview
    {
        [SerializeField] private ScrollRect _scrollRect = null;

        private TMP_Text text;

        public override void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider)
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

            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, Mathf.Max(text.renderedHeight, 0f));
        }
    }
}
