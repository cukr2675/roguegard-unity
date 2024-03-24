using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using TMPro;

namespace Objforming.Unity.RuntimeInspector
{
    public class LinkElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _label = null;

        private FormInspector inspector;
        private object value;

        public void Initialize(FormInspector inspector, string label, object value)
        {
            _label.text = $"<link>{label}</link>";
            this.inspector = inspector;
            this.value = value;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_label, eventData.position, null);
            if (linkIndex == -1) return;

            inspector.SetTarget(value);
        }
    }
}
