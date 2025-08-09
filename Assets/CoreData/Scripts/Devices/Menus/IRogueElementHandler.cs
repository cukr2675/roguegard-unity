using Lysionium;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IRogueElementHandler : IViewItemHandler
    {
        void GetRogueInfo(
            object item, MMgr manager, MArg arg,
            out object nameObj, ref Color color, ref Sprite icon, ref Color iconColor,
            ref int? stack, ref float? stars, ref string infoText1, ref string infoText2, ref bool equipeed);
    }
}
