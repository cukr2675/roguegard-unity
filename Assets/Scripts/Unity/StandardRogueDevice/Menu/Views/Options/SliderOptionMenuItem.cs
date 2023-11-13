using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace RoguegardUnity
{
    public class SliderOptionMenuItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private Slider _slider = null;

        public void Initialize(UnityAction<float> onValueChanged)
        {
            _slider.onValueChanged.AddListener(onValueChanged);
        }

        public void Open(string label, float value)
        {
            _label.text = label;
            _slider.value = value;
        }
    }
}
