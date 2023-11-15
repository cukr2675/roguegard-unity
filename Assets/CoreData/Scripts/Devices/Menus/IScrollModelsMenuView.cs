using Roguegard.Device;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IScrollModelsMenuView : IModelsMenuView
    {
        void ShowExitButton(IModelsMenuChoice exitItem);
    }
}
