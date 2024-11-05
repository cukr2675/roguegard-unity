using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class MessageController : MonoBehaviour
    {
        [SerializeField] private LogMenuView _logWindow = null;

        [SerializeField] private MessageBoxSubView _messageBox = null;

        private bool messageBoxIsVisible;
        private int showMessageTime;
        private const int messageTime = 3 * 60;

        internal void UpdateUI(SoundController soundController, int deltaTime)
        {
            if (messageBoxIsVisible && !_messageBox.MessageBox.IsInProgress)
            {
                showMessageTime += deltaTime;
                if (showMessageTime >= messageTime)
                {
                    _messageBox.Hide(false);
                    messageBoxIsVisible = false;
                }
            }
        }

        public void ShowMessage()
        {
            _messageBox.Show();
            messageBoxIsVisible = true;
        }

        private void AppendText(string text)
        {

            {
                _messageBox.MessageBox.Append(text);
                //_logWindow.Append(text);
                showMessageTime = 0;
                ShowMessage();
            }
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
            //if (talk)
            //{
            //    _talkWindow.Append(integer);
            //}
            //else
            //{
            //    _messageWindow.Append(integer);
            //    _logWindow.Append(integer);
            //    showMessageTime = 0;
            //}
        }

        public void AppendNumber(float number)
        {
            AppendText(number.ToString());
            //if (talk)
            //{
            //    _talkWindow.Append(number);
            //}
            //else
            //{
            //    _messageWindow.Append(number);
            //    _logWindow.Append(number);
            //    showMessageTime = 0;
            //}
        }

        public void AppendHorizontalRule()
        {
            AppendText("<link=\"HorizontalRule\"></link>");
            //if (talk)
            //{
            //    _talkWindow.AppendHorizontalRule();
            //}
            //else
            //{
            //    _messageWindow.AppendHorizontalRule();
            //    _logWindow.AppendHorizontalRule();
            //    showMessageTime = 0;
            //}
        }

        public void ClearText()
        {
            _messageBox.MessageBox.Clear();
            //_logWindow.Clear();
        }
    }
}
