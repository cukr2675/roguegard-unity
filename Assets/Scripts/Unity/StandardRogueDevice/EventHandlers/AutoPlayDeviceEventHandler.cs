using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal class AutoPlayDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly TouchController touchController;
        private readonly System.Action<RogueObj> setSubject;

        public AutoPlayDeviceEventHandler(TouchController touchController, System.Action<RogueObj> setSubject)
        {
            this.touchController = touchController;
            this.setSubject = setSubject;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            if (keyword == DeviceKw.StartAutoPlay && obj is RogueObj autoPlayObj)
            {
                // オートプレイ開始
                touchController.AutoPlayIsEnabled = true;
                setSubject(autoPlayObj);
                touchController.MenuOpen(autoPlayObj);
                return true;
            }
            return false;
        }
    }
}
