using Lysionium;
using Roguegard;
using System.Diagnostics;
using UnityEngine;

namespace RoguegardUnity
{
    public class MessageController
    {
        private readonly IMessageBoxSubview messageSubview;
        private readonly IMessageBoxSubview logSubview;

        private bool messageBoxIsVisible;
        private int showMessageTime;
        private const int messageTime = 3 * 60;

        public MessageController(IMessageBoxSubview messageSubview, IMessageBoxSubview logSubview)
        {
            this.messageSubview = messageSubview;
            this.logSubview = logSubview;
        }

        internal void UpdateUI(int deltaTime)
        {
            if (messageBoxIsVisible && !messageSubview.IsInProgress)
            {
                showMessageTime += deltaTime;
                if (showMessageTime >= messageTime)
                {
                    messageSubview.Hide(false);
                    messageBoxIsVisible = false;
                }
            }
        }

        public void ShowMessage()
        {
            messageSubview.Show();
            messageBoxIsVisible = true;
        }

        private void AppendText(string text)
        {
            messageSubview.Append(text);
            logSubview.Append(text);
            showMessageTime = 0;
            ShowMessage();
        }

        private void AppendObj(RogueObj player, RogueObj obj)
        {
            var color = StandardRogueDeviceUtility.GetColor(player, obj);
            var rgba = ColorUtility.ToHtmlStringRGBA(color);
            AppendText("<color=#");
            AppendText(rgba);
            AppendText(">");
            AppendText(obj.Main.InfoSet.Name);
            AppendText("</color>");
        }

        public void Append(RogueObj player, object obj, StackTrace stackTrace)
        {
            if (obj is string text)
            {
                if (text.StartsWith(':'))
                {
                    AppendText(text);
                }
                else
                {
                    AppendText(text);
                }
            }
            else if (obj is RogueObj rogueObj)
            {
                AppendObj(player, rogueObj);
            }
            else if (obj == DeviceKw.HorizontalRule)
            {
                AppendHorizontalRule();
            }
            else if (obj is IRogueDescribable describable)
            {
                AppendText(describable.Name);
            }
            else
            {
                UnityEngine.Debug.LogError(stackTrace.ToString());
                throw new System.InvalidOperationException();
            }
        }

        public void AppendInteger(int integer)
        {
            AppendText(integer.ToString());
        }

        public void AppendNumber(float number)
        {
            AppendText(number.ToString());
        }

        public void AppendHorizontalRule()
        {
            AppendText("<link=\"HorizontalRule\"></link>");
        }

        public void ClearText()
        {
            messageSubview.Clear();
            logSubview.Clear();
        }
    }
}
