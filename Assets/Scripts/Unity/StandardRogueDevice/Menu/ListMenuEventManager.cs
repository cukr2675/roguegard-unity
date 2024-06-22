using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal class ListMenuEventManager
    {
        private readonly MessageController messageController;
        private readonly SoundController soundController;
        private readonly WaitTimer waitTimer;

        public RogueObj MenuSubject { get; set; }

        public bool Wait => soundController.Wait || waitTimer.Wait;

        public ListMenuEventManager(MessageController messageController, SoundController soundController)
        {
            this.messageController = messageController;
            this.soundController = soundController;
            waitTimer = new WaitTimer();
        }

        public void UpdateUI(int deltaTime)
        {
            messageController.UpdateUI(soundController, deltaTime);
            waitTimer.UpdateTimer(deltaTime);
        }

        public void ClearText()
        {
            messageController.ClearText();
        }

        public void Append(RogueObj player, object obj, System.Diagnostics.StackTrace stackTrace)
        {
            messageController.Append(player, obj, stackTrace);
        }

        public void AppendInteger(int integer)
        {
            messageController.AppendInteger(integer);
        }

        public void AppendNumber(float number)
        {
            messageController.AppendNumber(number);
        }

        public void Add(IKeyword keyword, int integer = 0, float number = 0f, object obj = null)
        {
            if (keyword == null) throw new System.ArgumentNullException(nameof(keyword));

            if (keyword == DeviceKw.AppendText)
            {
                if (obj is string text) { Append(MenuSubject, StandardRogueDeviceUtility.Localize(text), null); }
                else if (obj != null) { Append(MenuSubject, obj, null); }
                else if (number == 0f) { AppendInteger(integer); }
                else { AppendNumber(number); }
                return;
            }
            if (keyword == DeviceKw.StartTalk)
            {
                messageController.StartTalk();
                return;
            }
            if (keyword == DeviceKw.WaitEndOfTalk)
            {
                messageController.WaitEndOfTalk();
                return;
            }
            if (keyword == DeviceKw.EnqueueSE || keyword == DeviceKw.EnqueueSEAndWait)
            {
                soundController.Play((IKeyword)obj, keyword == DeviceKw.EnqueueSEAndWait);
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
