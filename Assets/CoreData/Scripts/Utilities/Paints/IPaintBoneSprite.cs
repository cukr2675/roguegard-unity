using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using RuntimeDotter;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IPaintBoneSprite
    {
        void AddTo(EffectableBoneSpriteTable table, Color mainColor, Spanning<ShiftableColor> palette);

        Sprite GetIcon(Spanning<ShiftableColor> palette);

        IPaintBoneSprite Clone();
    }
}
