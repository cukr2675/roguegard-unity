using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.Events;
using TMPro;

namespace RoguegardUnity
{
    public class DropdownOptionMenuItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private TMP_Dropdown _dropdown = null;

        public void Initialize(UnityAction<int> onValueChanged)
        {
            _dropdown.onValueChanged.AddListener(onValueChanged);
        }

        internal void Open(string label, string[] items, int selectedIndex)
        {
            _label.text = label;
            _dropdown.options = items.Select(x => new TMP_Dropdown.OptionData(x)).ToList();
            _dropdown.value = selectedIndex;
        }
    }
}
