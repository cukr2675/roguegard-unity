using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class MessageController : MonoBehaviour
    {
        [SerializeField] private MessageWindow _messageWindow = null;
        [SerializeField] private LogMenuView _logWindow = null;
        [SerializeField] private TalkWindow _talkWindow = null;

        private int showMessageTime;
        private const int messageTime = 3 * 60;

        private bool lastTypewriting;

        public bool IsScrollingNow => _messageWindow.IsScrollingNow || _talkWindow.IsScrollingNow;

        public bool IsTalkingNow => _talkWindow.IsScrollingNow;

        public ModelsMenuView LogView => _logWindow;
        public ModelsMenuView TalkView => _talkWindow;

        internal void UpdateUI(SoundController soundController, int deltaTime)
        {
            _messageWindow.UpdateUI(deltaTime);
            if (_messageWindow.IsShow && !_messageWindow.IsScrollingNow)
            {
                showMessageTime += deltaTime;
                if (showMessageTime >= messageTime)
                {
                    _messageWindow.Show(false);
                }
            }

            _talkWindow.UpdateUI(deltaTime);
            if (lastTypewriting != _talkWindow.IsTypewritingNow)
            {
                if (_talkWindow.IsTypewritingNow)
                {
                    soundController.PlayLoop(DeviceKw.StartTalk);
                }
                else
                {
                    soundController.SetLastLoop(DeviceKw.StartTalk);
                }
                lastTypewriting = _talkWindow.IsTypewritingNow;
            }
        }

        public void ShowMessage(bool show)
        {
            _messageWindow.Show(show);
        }

        public void StartTalk()
        {
            _messageWindow.Show(false);
            _talkWindow.StartTalk();
        }

        public void WaitEndOfTalk()
        {
            _talkWindow.WaitEndOfTalk();
        }

        private void AppendText(string text)
        {
            if (_talkWindow.IsShow)
            {
                _talkWindow.Append(text);
            }
            else
            {
                _messageWindow.Append(text);
                _logWindow.Append(text);
                showMessageTime = 0;
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
            if (_talkWindow.IsShow)
            {
                _talkWindow.Append(integer);
            }
            else
            {
                _messageWindow.Append(integer);
                _logWindow.Append(integer);
                showMessageTime = 0;
            }
        }

        public void AppendNumber(float number)
        {
            if (_talkWindow.IsShow)
            {
                _talkWindow.Append(number);
            }
            else
            {
                _messageWindow.Append(number);
                _logWindow.Append(number);
                showMessageTime = 0;
            }
        }

        public void AppendHorizontalRule()
        {
            if (_talkWindow.IsShow)
            {
                _talkWindow.AppendHorizontalRule();
            }
            else
            {
                _messageWindow.AppendHorizontalRule();
                _logWindow.AppendHorizontalRule();
                showMessageTime = 0;
            }
        }

        public void ClearText()
        {
            _messageWindow.Clear();
            _logWindow.Clear();
            _talkWindow.Clear();
        }
    }
}
