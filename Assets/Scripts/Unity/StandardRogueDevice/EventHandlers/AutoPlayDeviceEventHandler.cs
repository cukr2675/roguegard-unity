using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal class AutoPlayDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly TouchController touchController;
        private readonly System.Action<RogueObj> setTargetObj;

        public AutoPlayDeviceEventHandler(TouchController touchController, System.Action<RogueObj> setTargetObj)
        {
            this.touchController = touchController;
            this.setTargetObj = setTargetObj;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            if (keyword == DeviceKw.StartAutoPlay && obj is RogueObj autoPlayObj)
            {
                // �I�[�g�v���C�J�n
                touchController.AutoPlayIsEnabled = true;
                setTargetObj(autoPlayObj);
                touchController.MenuOpen(autoPlayObj);
                return true;
            }
            return false;
        }
    }
}
