using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using TMPro;
using Roguegard;

namespace RoguegardUnity
{
    public class CaptionWindow : MenuWindow
    {
        [SerializeField] private TMP_Text _text = null;

        private readonly StringBuilder stringBuilder = new StringBuilder();

        public void Log(IRogueDescription description)
        {
            var text = description.Caption;
            if (text == null)
            {
                var name = description.Name;
                if (name.StartsWith(':'))
                {
                    text = StandardRogueDeviceUtility.Localize($"{name}::c");
                }
            }

            stringBuilder.Clear();
            stringBuilder.Append(text);
            _text.SetText(stringBuilder);
            Show(true);
        }
    }
}
