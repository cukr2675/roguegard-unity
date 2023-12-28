using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal class AutoPlayDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;
        private readonly TouchController touchController;
        private readonly System.Action<RogueObj> setSubject;

        public AutoPlayDeviceEventHandler(
            StandardRogueDeviceComponentManager componentManager, TouchController touchController, System.Action<RogueObj> setSubject)
        {
            this.componentManager = componentManager;
            this.touchController = touchController;
            this.setSubject = setSubject;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            if (keyword == DeviceKw.StartAutoPlay && obj is RogueObj autoPlayObj)
            {
                // オートプレイ開始
                setSubject(autoPlayObj);
                touchController.MenuOpen(autoPlayObj, true);
                return true;
            }
            return false;
        }

        public void StopAutoPlay()
        {
            setSubject(componentManager.Player);
            touchController.MenuOpen(componentManager.Player, false);
        }
    }
}
