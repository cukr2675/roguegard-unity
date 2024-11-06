using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class CaptionWindow : ElementsView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private TMP_Text _text = null;

        //public override CanvasGroup CanvasGroup => _canvasGroup;

        //public override void OpenView<T>(
        //    IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //{
        //    if (arg.Other is IRogueDescription description)
        //    {
        //        ShowCaption(description);
        //    }
        //    else
        //    {
        //        ShowCaption(arg.Other?.ToString() ?? "");
        //    }
        //}

        //public override float GetPosition()
        //{
        //    return 0f;
        //}

        //public override void SetPosition(float position)
        //{
        //}

        public static string ShowCaption(string text)
        {
            if (text.StartsWith(':') && !StandardRogueDeviceUtility.TryLocalize(text, out text))
            {
                text = "";
            }
            return text;
        }

        public static string ShowCaption(IRogueDescription description)
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
            return text;
        }
    }
}
