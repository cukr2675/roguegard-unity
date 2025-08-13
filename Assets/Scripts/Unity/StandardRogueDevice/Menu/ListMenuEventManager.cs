using Lysionium.Audio;
using Roguegard;
using UnityEngine;

namespace RoguegardUnity
{
    internal class ListMenuEventManager
    {
        private readonly MessageController messageController;
        private readonly WebOtherAudioPlayHandler audioPlayHandler;
        private readonly WaitTimer waitTimer;

        public RogueObj MenuSubject { get; set; }

        public bool Wait => audioPlayHandler.Wait || waitTimer.Wait;

        public ListMenuEventManager(MessageController messageController, WebOtherAudioPlayHandler audioPlayHandler)
        {
            this.messageController = messageController;
            this.audioPlayHandler = audioPlayHandler;
            waitTimer = new WaitTimer();
        }

        public void UpdateUI(int deltaTime)
        {
            messageController.UpdateUI(deltaTime);
            waitTimer.UpdateTimer(deltaTime);
        }

        public void ClearText()
        {
            messageController.ClearText();
        }

        public void AppendTextObj(RogueObj player, object obj, System.Diagnostics.StackTrace stackTrace)
        {
            messageController.Append(player, obj, stackTrace);
        }

        public void Add(IKeyword keyword, int integer = 0, float number = 0f, object obj = null, System.Diagnostics.StackTrace stackTrace = null)
        {
            if (keyword == null) throw new System.ArgumentNullException(nameof(keyword));

            if (keyword == DeviceKw.AppendText)
            {
                if (obj is string text) { AppendTextObj(MenuSubject, StandardRogueDeviceUtility.Localize(text), stackTrace); }
                else if (obj != null) { AppendTextObj(MenuSubject, obj, stackTrace); }
                else if (number == 0f) { messageController.AppendInteger(integer); }
                else { messageController.AppendNumber(number); }
                return;
            }
            if (keyword == DeviceKw.EnqueueSE || keyword == DeviceKw.EnqueueSEAndWait)
            {
                audioPlayHandler.Play(((IKeyword)obj).Name, keyword == DeviceKw.EnqueueSEAndWait);
                return;
            }
            if (keyword == DeviceKw.EnqueueWaitSeconds)
            {
                waitTimer.Start(number);
                return;
            }

            Debug.LogError($"{keyword.Name} に対応するキーワードが見つかりません。（obj: {obj}）");
        }
    }
}
