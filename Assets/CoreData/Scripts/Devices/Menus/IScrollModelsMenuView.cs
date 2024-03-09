using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public interface IScrollModelsMenuView : IModelsMenuView
    {
        void ShowExitButton(IModelsMenuChoice exitItem);
    }
}
