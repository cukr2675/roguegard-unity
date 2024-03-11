using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace RoguegardUnity
{
    public class ModelsMenuViewOptionCheckBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private Toggle _toggle = null;

        public void Initialize(string label, bool value, UnityAction<bool> onValueChanged)
        {
            _label.text = label;
            _toggle.SetIsOnWithoutNotify(value);
            _toggle.onValueChanged.AddListener(onValueChanged);
        }
    }
}
