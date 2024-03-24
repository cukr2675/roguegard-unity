using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace Objforming.Unity.RuntimeInspector
{
    public class ToggleElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private Toggle _toggle = null;

        private FormInspector inspector;
        private ElementValueGetter getter;

        public void Initialize(
            FormInspector inspector, string label, ElementValueGetter getter, ElementValueSetter setter,
            bool interactable = true)
        {
            this.inspector = inspector;
            _label.text = label;
            this.getter = getter;
            UpdateValue();
            _toggle.onValueChanged.AddListener(x => setter(x));
            _toggle.interactable = interactable;

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

                yield return inspector.UpdateCoroutineWait;
            }
        }
    }
}
