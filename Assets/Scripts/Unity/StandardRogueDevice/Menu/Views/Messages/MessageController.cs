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
        [SerializeField] private MessageWindow _messageWindow = null;
        [SerializeField] private LogMenuView _logWindow = null;
        [SerializeField] private TalkWindow _talkWindow = null;

        [SerializeField] private MessageBoxSubView _messageBox = null;
        [SerializeField] private MessageBoxSubView _speechBox = null;

        private int showMessageTime;
        private const int messageTime = 3 * 60;

        private bool lastTypewriting;

        private bool talk;

        public bool IsScrollingNow => _messageBox.MessageBox.IsInProgress || _speechBox.MessageBox.IsInProgress;

        public bool IsTalkingNow => _speechBox.MessageBox.IsInProgress;

        public ElementsView LogView => _logWindow;
        public ElementsView TalkView => _talkWindow;

        internal void UpdateUI(SoundController soundController, int deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _speechBox.MessageBox.InvokeReachHiddenLink("Submit");
            }


            //_messageWindow.UpdateUI(deltaTime);
            //if (_messageWindow.IsShow && !_messageWindow.IsScrollingNow)
            //{
            //    showMessageTime += deltaTime;
            //    if (showMessageTime >= messageTime)
            //    {
            //        _messageWindow.Show(false);
            //    }
            //}

            //_talkWindow.UpdateUI(deltaTime);
            //if (lastTypewriting != _talkWindow.IsTypewritingNow)
            //{
            //    if (_talkWindow.IsTypewritingNow)
            //    {
            //        soundController.PlayLoop(DeviceKw.StartTalk);
            //    }
            //    else
            //    {
            //        soundController.SetLastLoop(DeviceKw.StartTalk);
            //    }
            //    lastTypewriting = _talkWindow.IsTypewritingNow;
            //}
        }

        public void ShowMessage(bool show)
        {
            _messageBox.Show();
            //_messageWindow.Show(show);
        }

        public void StartTalk()
        {
            _speechBox.Show();
            talk = true;
            _speechBox.DoScheduledAfterCompletion((manager, arg) => talk = false);
            //_messageWindow.Show(false);
            //_talkWindow.StartTalk();
        }

        public void WaitEndOfTalk()
        {
            //_talkWindow.WaitEndOfTalk();
        }

        private void AppendText(string text)
        {
            if (talk)
            {
                _speechBox.MessageBox.Append(text);
            }
            else
            {
                _messageBox.MessageBox.Append(text);
                //_logWindow.Append(text);
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
            _speechBox.MessageBox.Clear();
        }
    }
}
