using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using Lysionium;
using Roguegard;

namespace RoguegardUnity
{
    public class MessageController
    {
        private readonly MessageBoxSubview messageSubview;
        private readonly MessageBoxSubview logSubview;

        private bool messageBoxIsVisible;
        private int showMessageTime;
        private const int messageTime = 3 * 60;

        public MessageController(StandardSubviewTable table)
        {
            messageSubview = table.MessageBox;
            logSubview = table.LongMessage;
        }

        internal void UpdateUI(int deltaTime)
        {
            if (messageBoxIsVisible && !messageSubview.MessageBox.IsInProgress)
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
            messageSubview.MessageBox.Append(text);
            logSubview.MessageBox.Append(text);
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
            else if (obj is IRogueDescription description)
            {
                AppendText(description.Name);
            }
            else
            {
                UnityEngine.Debug.LogError(stackTrace.ToString());
                throw new RogueException();
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
            messageSubview.MessageBox.Clear();
            logSubview.MessageBox.Clear();
        }
    }
}
