using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using TMPro;

namespace Objforming.Unity.RuntimeInspector
{
    public class InputElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private TMP_InputField _inputField = null;

        private FormInspector inspector;
        private ElementValueGetter getter;

        public void Initialize(
            FormInspector inspector, string label, ElementValueGetter getter, UnityAction<string> setter,
            TMP_InputField.ContentType contentType, bool interactable = true)
        {
            this.inspector = inspector;
            _label.text = label;
            this.getter = getter;
            UpdateValue();
            _inputField.onEndEdit.AddListener(setter);
            _inputField.contentType = contentType;
            _inputField.interactable = interactable;

            StartCoroutine(UpdateCoroutine());

        }

        private void UpdateValue()
        {
            var value = getter();
            _inputField.text = value?.ToString();
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
