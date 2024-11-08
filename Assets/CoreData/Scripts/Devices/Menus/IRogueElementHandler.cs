using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public interface IRogueElementHandler : IElementHandler
    {
        void GetRogueInfo(
            object element, MMgr manager, MArg arg,
            out object nameObj, ref Color color, ref Sprite icon, ref Color iconColor,
            ref int? stack, ref float? stars, ref string infoText1, ref string infoText2, ref bool equipeed);
    }
}
