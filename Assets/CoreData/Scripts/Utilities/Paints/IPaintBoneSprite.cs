using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;

namespace Roguegard
{
    [ObjectFormer.RequireRelationalComponent]
    public interface IPaintBoneSprite
    {
        void AddTo(AffectableBoneSpriteTable table, Color mainColor, Spanning<ShiftableColor> palette);

        Sprite GetIcon(Spanning<ShiftableColor> palette);

        IPaintBoneSprite Clone();
    }
}
