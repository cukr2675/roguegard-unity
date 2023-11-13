using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace RoguegardUnity
{
    public class MessageWindow : MenuWindow
    {
        [SerializeField] private MessageText _text = null;

        public bool IsScrollingNow => _text.IsScrollingNow;

        public void Append(string text)
        {
            _text.Append(text);
            Show(true);
        }

        public void Append(int integer)
        {
            _text.Append(integer);
            Show(true);
        }

        public void Append(float number)
        {
            _text.Append(number);
            Show(true);
        }

        public void AppendHorizontalRule()
        {
            _text.AppendHorizontalRule();
        }

        public void Clear()
        {
            _text.Clear();
            Show(false);
        }

        public void UpdateUI(int deltaTime)
        {
            _text.UpdateUI(deltaTime);
        }
    }
}
