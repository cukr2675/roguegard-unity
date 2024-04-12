using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    /// <summary>
    /// ���E�ɓ������A�����̎��Ԃł͂Ȃ����O�K�����E�̎��Ԃ��R�[���o�b�N����C�x���g�n���h���B
    /// </summary>
    internal class DateTimeCallbackEventHandler : IStandardRogueDeviceEventHandler
    {
        private bool overridesDateTime;
        private System.DateTime dateTime;

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            if (keyword == DeviceKw.GetDateTimeUtc)
            {
                var callback = (RogueDateTime.Callback)obj;
                if (overridesDateTime)
                {
                    callback(dateTime);
                }
                else
                {
                    callback(System.DateTime.UtcNow);
                }
                return true;
            }
            return false;
        }

        public void SetInGameTime(System.DateTime dateTime)
        {
            overridesDateTime = true;
            this.dateTime = dateTime;
        }

        public void UseRealTime()
        {
            overridesDateTime = false;
        }
    }
}
