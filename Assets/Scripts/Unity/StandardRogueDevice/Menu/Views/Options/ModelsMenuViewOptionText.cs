using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using TMPro;

namespace RoguegardUnity
{
    public class ModelsMenuViewOptionText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private TMP_InputField _inputField = null;

        public void Initialize(string label, string value, UnityAction<string> onValueChanged)
        {
            _label.text = label;
            _inputField.SetTextWithoutNotify(value);
            _inputField.onValueChanged.AddListener(onValueChanged);
        }
    }
}
