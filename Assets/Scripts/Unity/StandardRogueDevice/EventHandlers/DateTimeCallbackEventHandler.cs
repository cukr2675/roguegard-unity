using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    /// <summary>
    /// 世界に同期中、現実の時間ではなくログガル世界の時間をコールバックするイベントハンドラ。
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
