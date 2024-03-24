using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Objforming.Unity.RuntimeInspector
{
    public class ButtonElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private Button _button = null;

        public void Initialize(string text, UnityAction action, bool interactable = true)
        {
            _text.text = text;
            _button.onClick.AddListener(action);
            _button.interactable = interactable;
        }
    }
}
