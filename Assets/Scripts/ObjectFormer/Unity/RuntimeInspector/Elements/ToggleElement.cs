using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace ObjectFormer.Unity.RuntimeInspector
{
    public class ToggleElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private Toggle _toggle = null;

        private ElementValueGetter getter;
        private object updateCoroutineWait;

        public void Initialize(string label, ElementValueGetter getter, ElementValueSetter setter, object updateCoroutineWait)
        {
            _label.text = label;
            this.getter = getter;
            this.updateCoroutineWait = updateCoroutineWait;
            UpdateValue();
            _toggle.onValueChanged.AddListener(x => setter(x));

            StartCoroutine(UpdateCoroutine());

        }

        private void UpdateValue()
        {
            var value = getter();
            _toggle.SetIsOnWithoutNotify((bool)value);
        }

        private IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                UpdateValue();

                yield return updateCoroutineWait;
            }
        }
    }
}
