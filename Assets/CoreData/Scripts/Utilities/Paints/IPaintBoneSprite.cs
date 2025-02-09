using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;
using RuntimeDotter;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IPaintBoneSprite
    {
        void AddTo(OchalikeMorph ochalikeMorph, Color mainColor, Spanning<ShiftableColor> palette);

        Sprite GetIcon(Spanning<ShiftableColor> palette);

        IPaintBoneSprite Clone();
    }
}
