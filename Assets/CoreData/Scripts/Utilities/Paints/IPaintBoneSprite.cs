using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using RuntimeDotter;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IPaintBoneSprite
    {
        void AddTo(AffectableBoneSpriteTable table, Color mainColor, Spanning<ShiftableColor> palette);

        Sprite GetIcon(Spanning<ShiftableColor> palette);

        IPaintBoneSprite Clone();
    }
}
