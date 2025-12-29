using Lysionium.Views;
using Roguegard.Device;
using TMPro;
using UnityEngine;

namespace RoguegardUnity
{
    public class TextEditorSubview : Subview, ITextEditorElementsSubview
    {
        [SerializeField] private TMP_InputField _inputField = null;

        public string Text { get => _inputField.text; set => _inputField.SetTextWithoutNotify(value); }
    }
}
