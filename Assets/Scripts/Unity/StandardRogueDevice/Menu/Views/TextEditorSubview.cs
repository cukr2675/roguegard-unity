using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Lysionium;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class TextEditorSubview : Subview, ITextEditorElementsSubview
    {
        [SerializeField] private TMP_InputField _inputField = null;

        public string Text { get => _inputField.text; set => _inputField.SetTextWithoutNotify(value); }

        public override void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider)
            => throw new System.NotSupportedException();
    }
}
