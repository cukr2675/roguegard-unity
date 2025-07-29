using TMPro;
using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Items/LUI Label View Item")]
    [RequireComponent(typeof(TMP_Text))]
    public class LabelViewItem : ViewItem
    {
        private TMP_Text text;

        private void Awake()
        {
            TryGetComponent(out text);
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            text.text = Manager.Localize(name);
        }
    }
}
