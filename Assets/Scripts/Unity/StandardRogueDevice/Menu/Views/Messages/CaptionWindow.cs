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

        public void ShowCaption(string text)
        {
            if (text.StartsWith(':') && !StandardRogueDeviceUtility.TryLocalize(text, out text))
            {
                text = "";
            }
            _text.SetText(text);
            Show(true);
        }

        public void ShowCaption(IRogueDescription description)
        {
            var text = description.Caption;
            if (text == null)
            {
                var name = description.Name;
                if (name.StartsWith(':'))
                {
                    if (!StandardRogueDeviceUtility.TryLocalize($"{name}::c", out text))
                    {
                        text = "";
                    }
                }
            }

            _text.SetText(text);
            Show(true);
        }
    }
}
