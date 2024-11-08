using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class TextEditorSubView : ElementsSubView, ITextEditorElementsSubView
    {
        [SerializeField] private TMP_InputField _inputField = null;

        public string Text { get => _inputField.text; set => _inputField.SetTextWithoutNotify(value); }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
            => throw new System.NotSupportedException();
    }
}
