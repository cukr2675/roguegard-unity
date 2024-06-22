using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace RoguegardUnity
{
    public class OptionsViewSlider : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private Slider _slider = null;

        public void Initialize(string label, float value, UnityAction<float> onValueChanged)
        {
            _label.text = label;
            _slider.SetValueWithoutNotify(value);
            _slider.onValueChanged.AddListener(onValueChanged);
        }
    }
}
